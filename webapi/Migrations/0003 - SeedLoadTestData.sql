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

create or replace function true_random() returns double precision
as $$
BEGIN
	RETURN 
		select random() as v1
		FROM generate_series(0,1);
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION random_timestamp_between_dates(
    start_date timestamp,
	end_date timestamp
)
RETURNS timestamp AS $$
DECLARE

BEGIN

    RETURN end_date +
			random() * (start_date - end_date);
END
$$ LANGUAGE plpgsql;



create extension if not exists "uuid-ossp";

-- insert 1k tenants
with simul_tenants_data as (
	select
		uuid_generate_v4() as tenant_id,
		random_timestamp_between_dates('2010-10-01 00:00:00'::timestamp, '2020-10-01 00:00:00'::timestamp) as created_at,
		concat('Tenant ', i::text) as name, 
		random_choice(array['Open', 'Lockdown', 'Out of Business']) as status
	from 
		generate_series(1,1000) as series(i) 
) 
insert into 
	public.tenants (tenant_id, created_at, name, status)
select 
	*
from
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
	return 1;
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

select create_books_sequence_for_tenants();

GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO tenancy_users;


-- authors seed setup

create temp table if not exists first_names(
	id int PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	first_name text);
copy
	first_names(first_name)
FROM 
    '/tmp/30k_first_names.csv';

create temp table if not exists last_names(
	id int PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	last_name text);
copy
	last_names(last_name)
FROM 
    '/tmp/30k_last_names.csv';

create temp table if not exists countries(
	id int PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	name text
	);
copy
	countries(name)
FROM 
    '/tmp/countries.csv';

CREATE EXTENSION if not exists tsm_system_rows;


CREATE OR REPLACE FUNCTION random_country()
RETURNS text AS $$
DECLARE

BEGIN

    RETURN 
		(select
			name
		from
			countries
		order by
			true_random()
		limit 1);
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION random_author_name()
RETURNS text AS $$
DECLARE
	random_row_id int := round(random()*(30000-1) + 1);
BEGIN

    RETURN 
		(select
			first_name
		from 
			first_names
		where
			id = random_row_id)
		 ||
		 ' '
		 ||
		(select
			last_name
		from
			last_names 
		where
			id = random_row_id);
END
$$ LANGUAGE plpgsql;


-- insert 1m authors	
create or replace function create_1k_authors_rows()
returns table(created_at timestamp, bith_date date, name text, country text) as $$
begin

	return query
			select
				random_timestamp_between_dates('2020-01-01 00:00:00'::timestamp, '2020-10-01 00:00:00'::timestamp) as created_at,
				random_timestamp_between_dates('1901-01-01 00:00:00'::timestamp, '2010-01-01 00:00:00'::timestamp)::date as birth_date,
				random_author_name() as name, 
				random_country() as country
			from 
				generate_series(1,1000) as series(i); 
end;
$$ LANGUAGE plpgsql;	


CREATE or REPLACE PROCEDURE bulk_load_authors()
    LANGUAGE plpgsql
    AS $$
    DECLARE
	 start_num int;
    BEGIN
		for r in 1..1000 loop
				insert into 
					public.authors (created_at, birth_date, name, country)
				select 
					*
				from
					create_1k_authors_rows();
				COMMIT;
		end loop;
    END;
	$$;
	
	
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

-- insert 100m books



CREATE OR REPLACE FUNCTION random_tenant_id()
RETURNS uuid AS $$
DECLARE
	random_row_id int := round(random()*(1000-1) + 1);
BEGIN

    RETURN 
		(select
			tenant_id
		from
			public.tenants 
		where 
			tenant_number = random_row_id);
END
$$ LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION random_author_id()
	RETURNS double precision AS $$
BEGIN
	RETURN select round(random()*(1000000-1) + 1);
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION random_genre_id()
	RETURNS double precision AS $$
BEGIN
	RETURN select round(random()*(56-1) + 1);
END
$$ LANGUAGE plpgsql;


create or replace function create_1k_books_rows(start_index int)
returns 
table(tenant_id uuid, 
	  book_id uuid, 
	  book_number int, 
	  created_at timestamp, 
	  title text, 
	  year date, 
	  author_id int, 
	  genre_id int) 
as $$
begin

	return query
			select
				random_tenant_id() as tenant_id,
				uuid_generate_v4() as book_id,
				2147483646 as book_number,
				random_timestamp_between_dates('2020-01-01 00:00:00'::timestamp, '2020-10-01 00:00:00'::timestamp) as created_at,
				concat('The Theory of ', i::text) as title, 
				random_timestamp_between_dates('1910-01-01 00:00:00'::timestamp, '2020-10-01 00:00:00'::timestamp)::date as year,
				random_author_id()::int as author_id,	
				random_genre_id()::int as genre_id
			from 
				generate_series(start_index,start_index+1000) as series(i); 
end;
$$ LANGUAGE plpgsql;	



CREATE or REPLACE PROCEDURE bulk_load_1m_books()
    LANGUAGE plpgsql
    AS $$
    DECLARE
     r RECORD;
	 start_num int;
    BEGIN
		for r in 1..100000 loop
			if (r = 1) then start_num := 1; else start_num := r+1000; end if;
				insert into 
					public.books (tenant_id, book_id, book_number, created_at, title, year, author_id, genre_id)
				select 
					*
				from
					create_1k_books_rows(start_num);
				COMMIT; -- requires pg >= 12
		end loop;
    END;
	$$;
	
	
CALL bulk_load_1m_books();

