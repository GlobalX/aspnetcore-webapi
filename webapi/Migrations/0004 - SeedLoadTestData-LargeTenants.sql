CREATE OR REPLACE FUNCTION create_large_tenant(provided_tenant_id text, tenant_number int)
RETURNS 
TABLE(tenant_id uuid, 
	  created_at timestamp, 
	  name text, 
	  status text) 
AS $$
BEGIN
	RETURN QUERY
			SELECT
				provided_tenant_id::uuid AS tenant_id,
				random_timestamp_between_dates('2010-10-01 00:00:00'::timestamp, '2020-10-01 00:00:00'::timestamp) AS created_at,
				concat('Large Tenant ', tenant_number::text) AS name, 
				random_choice(array['Open', 'Lockdown', 'Out of Business']) AS status
			FROM 
				generate_series(1,1) AS series(i) 	;	 
END;
$$ LANGUAGE plpgsql;	


CREATE or REPLACE PROCEDURE load_large_tenants()
AS $$
DECLARE
    r RECORD;
    start_num int;
	current_tenant_index int := 1;
	large_tenants text[]  := '{"9D69218D-83A3-47AF-9EAC-00BF38C51497", "260B82BB-18DF-4BC2-B36D-B7DF07DEF6E8"}';
BEGIN
	FOR current_tenant_index IN 1..array_length(large_tenants, 1) LOOP
				INSERT INTO 
					public.tenants (tenant_id, created_at, name, status)
				SELECT 
					*
				FROM
					create_large_tenant(large_tenants[current_tenant_index], current_tenant_index);
				COMMIT; -- requires pg >= 12
	END LOOP;
END;
$$ LANGUAGE plpgsql;
	
	
CALL load_large_tenants();

CREATE OR REPLACE FUNCTION create_books_sequence_for_large_tenants()
RETURNS boolean AS $$
DECLARE
	current_tenant_id uuid;
BEGIN
	FOR current_tenant_id IN SELECT tenant_id FROM tenants where tenant_id IN ('9D69218D-83A3-47AF-9EAC-00BF38C51497', '260B82BB-18DF-4BC2-B36D-B7DF07DEF6E8')  LOOP
		--RAISE NOTICE 'Creating books sequence for %', current_tenant_id::text;
		PERFORM create_tenant_books_sequence(current_tenant_id);
	END LOOP;
	RETURN 1;
END;
$$ LANGUAGE plpgsql;

SELECT create_books_sequence_for_large_tenants();




CREATE OR REPLACE FUNCTION create_1k_books_rows_with_tenant_id(start_index int, provided_tenant_id text)
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
				provided_tenant_id::uuid AS tenant_id,
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


-- load 10m books in batches of 1000 for each of the large tenants
CREATE or REPLACE PROCEDURE bulk_load_large_tenant_books()
AS $$
DECLARE
    r RECORD;
    start_num int;
	current_tenant_index int := 1;
	large_tenants text[]  := '{"9D69218D-83A3-47AF-9EAC-00BF38C51497", "260B82BB-18DF-4BC2-B36D-B7DF07DEF6E8"}';
BEGIN
	FOR current_tenant_index IN 1..array_length(large_tenants, 1) LOOP
		FOR r IN 1..10000 LOOP
			IF (r = 1) THEN start_num := 1; ELSE start_num := r*1000; END IF;
				INSERT INTO 
					public.books (tenant_id, book_id, book_number, created_at, title, year, author_id, genre_id)
				SELECT 
					*
				FROM
					create_1k_books_rows_with_tenant_id(start_num, large_tenants[current_tenant_index]);
				COMMIT; -- requires pg >= 12
		END LOOP;
	END LOOP;
END;
$$ LANGUAGE plpgsql;
	
	
CALL bulk_load_large_tenant_books();

