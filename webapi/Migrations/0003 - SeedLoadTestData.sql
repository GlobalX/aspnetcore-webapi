CREATE OR REPLACE FUNCTION random_choice(
    choices text[]
)
RETURNS text AS $$
DECLARE
    size_ int;
BEGIN
    size_ = array_length(choices, 1);
    RETURN (choices)[floor(random()*size_)+1];
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION true_random() 
RETURNS double precision AS $$
BEGIN
	RETURN 
		select random() AS v1
		FROM generate_series(0,1);
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION random_timestamp_between_dates(
    start_date timestamp,
	end_date timestamp
)
RETURNS timestamp AS $$
BEGIN
    RETURN end_date + random() * (start_date - end_date);
END
$$ LANGUAGE plpgsql;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- insert 1k tenants
WITH simul_tenants_data AS (
	SELECT
		uuid_generate_v4() AS tenant_id,
		random_timestamp_between_dates('2010-10-01 00:00:00'::timestamp, '2020-10-01 00:00:00'::timestamp) AS created_at,
		concat('Tenant ', i::text) AS name, 
		random_choice(array['Open', 'Lockdown', 'Out of Business']) AS status
	FROM 
		generate_series(1,1000) AS series(i) 
) 
INSERT INTO 
	public.tenants (tenant_id, created_at, name, status)
SELECT 
	*
FROM
	simul_tenants_data;


-- create books sequences for a single tenant
CREATE OR REPLACE FUNCTION create_tenant_books_sequence(
    tenant_id uuid
)
RETURNS boolean AS $$
DECLARE
	sequence_name text  := 'books_' || replace(tenant_id::text, '-', '') || '_book_id_seq;';
BEGIN
	RAISE NOTICE 'tenant_id %', tenant_id::text;
    EXECUTE 'CREATE SEQUENCE IF NOT EXISTS ' || sequence_name;
	RETURN 1;
END
$$ LANGUAGE plpgsql;

-- create books sequences for all tenants
CREATE OR REPLACE FUNCTION create_books_sequence_for_tenants()
RETURNS boolean AS $$
DECLARE
	current_tenant_id uuid;
BEGIN
	FOR current_tenant_id IN SELECT tenant_id FROM tenants LOOP
		--RAISE NOTICE 'Creating books sequence for %', current_tenant_id::text;
		PERFORM create_tenant_books_sequence(current_tenant_id);
	END LOOP;
	RETURN 1;
END;
$$ LANGUAGE plpgsql;

SELECT create_books_sequence_for_tenants();

GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO tenancy_users;


-- authors seed setup
CREATE TEMP TABLE IF NOT EXISTS first_names(
	id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	first_name text);
COPY
	first_names(first_name)
FROM 
    '/tmp/30k_first_names.csv';

CREATE TEMP TABLE IF NOT EXISTS last_names(
	id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	last_name text);
COPY
	last_names(last_name)
FROM 
    '/tmp/30k_last_names.csv';

CREATE TEMP TABLE IF NOT EXISTS countries(
	id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	name text
	);
COPY
	countries(name)
FROM 
    '/tmp/countries.csv';

CREATE EXTENSION IF NOT EXISTS tsm_system_rows;


CREATE OR REPLACE FUNCTION random_country()
RETURNS text AS $$
BEGIN

    RETURN 
		(SELECT
			name
		FROM
			countries
		ORDER BY
			true_random()
		LIMIT 1);
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION random_author_name()
RETURNS text AS $$
DECLARE
	random_row_id int := round(random()*(30000-1) + 1);
BEGIN

    RETURN 
		(SELECT first_name FROM first_names WHERE id = random_row_id)
		 ||
		 ' '
		 ||
		(SELECT last_name FROM last_names WHERE id = random_row_id);
END
$$ LANGUAGE plpgsql;


-- insert 1m authors	
CREATE OR REPLACE FUNCTION create_1k_authors_rows()
RETURNS TABLE(created_at timestamp, bith_date date, name text, country text) AS $$
BEGIN
	RETURN QUERY
	    SELECT
	    	random_timestamp_between_dates('2020-01-01 00:00:00'::timestamp, '2020-10-01 00:00:00'::timestamp) AS created_at,
	    	random_timestamp_between_dates('1901-01-01 00:00:00'::timestamp, '2010-01-01 00:00:00'::timestamp)::date AS birth_date,
	    	random_author_name() AS name, 
	    	random_country() AS country
	    FROM 
	    	generate_series(1,1000) AS series(i); 
END;
$$ LANGUAGE plpgsql;	


CREATE or REPLACE PROCEDURE bulk_load_authors()
AS $$
DECLARE
    start_num int;
BEGIN
	FOR r IN 1..1000 loop
		INSERT INTO 
			public.authors (created_at, birth_date, name, country)
		SELECT 
			*
		FROM
			create_1k_authors_rows();
		COMMIT;
	END loop;
END;
$$ LANGUAGE plpgsql;

CALL bulk_load_authors();



-- books seed setup
CREATE OR REPLACE FUNCTION books_sequence_name(tenant_id uuid)
RETURNS text AS $$
DECLARE
	sequence_name text  := 'books_' || replace(tenant_id::text, '-', '') || '_book_id_seq';
BEGIN
    RETURN sequence_name;	
END
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION insert_book_number_trig() RETURNS trigger
   LANGUAGE plpgsql AS $$
BEGIN
   NEW.book_number := nextval(books_sequence_name(NEW.tenant_id));

   RETURN NEW;
END
$$;
 
CREATE TRIGGER ins_books_trig BEFORE INSERT ON books
   FOR EACH ROW
   EXECUTE PROCEDURE insert_book_number_trig();

-- books seed setup
CREATE OR REPLACE FUNCTION random_tenant_id()
RETURNS uuid AS $$
DECLARE
	random_row_id int := round(random()*(1000-1) + 1);
BEGIN

    RETURN 
		(SELECT
			tenant_id
		FROM
			public.tenants 
		WHERE 
			tenant_number = random_row_id);
END
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION random_author_id()
	RETURNS double precision AS $$
BEGIN
	RETURN SELECT round(random()*(1000000-1) + 1);
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION random_genre_id()
	RETURNS double precision AS $$
BEGIN
	RETURN SELECT round(random()*(56-1) + 1);
END
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION create_1k_books_rows(start_index int)
RETURNS 
TABLE(tenant_id uuid, 
	  book_id uuid, 
	  book_number int, 
	  created_at timestamp, 
	  title text, 
	  year date, 
	  author_id int, 
	  genre_id int) 
AS $$
BEGIN
	RETURN QUERY
			SELECT
				random_tenant_id() AS tenant_id,
				uuid_generate_v4() AS book_id,
				2147483646 AS book_number,
				random_timestamp_between_dates('2020-01-01 00:00:00'::timestamp, '2020-10-01 00:00:00'::timestamp) AS created_at,
				concat('The Theory of ', i::text) AS title, 
				random_timestamp_between_dates('1910-01-01 00:00:00'::timestamp, '2020-10-01 00:00:00'::timestamp)::date AS year,
				random_author_id()::int AS author_id,	
				random_genre_id()::int AS genre_id
			FROM 
				generate_series(start_index,start_index+1000) AS series(i); 
END;
$$ LANGUAGE plpgsql;	


-- load 100m books in batches of 1000
CREATE or REPLACE PROCEDURE bulk_load_1m_books()
AS $$
DECLARE
    r RECORD;
    start_num int;
BEGIN
	FOR r IN 1..100000 loop
		IF (r = 1) THEN start_num := 1; ELSE start_num := r*1000; END IF;
			INSERT INTO 
				public.books (tenant_id, book_id, book_number, created_at, title, year, author_id, genre_id)
			SELECT 
				*
			FROM
				create_1k_books_rows(start_num);
			COMMIT; -- requires pg >= 12
	END loop;
END;
$$ LANGUAGE plpgsql;
	
	
CALL bulk_load_1m_books();

