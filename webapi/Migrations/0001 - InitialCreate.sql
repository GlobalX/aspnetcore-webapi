-- Create the initial schema of the application

-- Create a role group which will contain the app user account. 
-- This role group allows management of access privileges in a single location. 
-- We will later apply select, insert, update, delete command access on all multi-tenanted tables for this role group.
DO $$
BEGIN
  CREATE ROLE tenancy_users WITH NOLOGIN;
  EXCEPTION WHEN DUPLICATE_OBJECT THEN
  RAISE NOTICE 'not creating role tenancy_users -- it already exists';
END
$$;

-- Create the app_user user
DO $$
BEGIN
  CREATE USER app_user WITH PASSWORD 'Welcome1';
  EXCEPTION WHEN DUPLICATE_OBJECT THEN
  RAISE NOTICE 'not creating user app_user -- it already exists';
  -- Add this user to the tenancy_group role
  GRANT tenancy_users TO app_user;
END
$$;

-- Table: Tenants
CREATE TABLE public.tenants (
    tenant_id uuid PRIMARY KEY,
    created_at timestamptz NOT NULL,
    name text,
    status text
);    

ALTER TABLE public.tenants ENABLE ROW LEVEL SECURITY;

GRANT SELECT ON TABLE public.tenants TO tenancy_users;

CREATE POLICY tenant_isolation_policy
    ON public.tenants
    AS PERMISSIVE
    FOR ALL
    TO public
    USING ((tenant_id = (current_setting('app.current_tenant'::text))::uuid));


-- Table: Authors
CREATE TABLE public.authors (
    author_id int PRIMARY KEY GENERATED ALWAYS AS IDENTITY, -- authors are global and not multi-tenanted; a sequential integer index is suitable. serial = 32bit int = 4.294 billion authors    
    created_at timestamptz NOT NULL,
    name text,
    country text,
    birth_date date
);

CREATE INDEX authors_name_idx ON public.authors USING btree (name);

-- Grant only select/insert/update/delete
-- may need to consider EXECUTE and USAGE later down the track
-- ALL shouldn't be used as Truncate is outside of RLS boundary, and ALTER TABLE can disable RLS, see: https://www.postgresql.org/docs/9.5/ddl-rowsecurity.html
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE public.authors TO tenancy_users;

-- Table: Genres
CREATE TABLE genres
(
    genre_id int PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    name text  
);

GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE public.genres TO tenancy_users;

-- Table: Books
CREATE TABLE public.books (
    book_id uuid NOT NULL, 
    book_number bigint NOT NULL,
    created_at timestamptz NOT NULL,
    title text,
    year timestamptz NOT NULL,
    author_id int REFERENCES public.authors(author_id),
    tenant_id uuid REFERENCES public.tenants(tenant_id),
    genre_id int REFERENCES public.genres(genre_id),
    PRIMARY KEY (tenant_id, book_number),  
    UNIQUE (book_id)
);

-- Create additional indexes that will speed up expected queries
CREATE INDEX books_tenant_id_fkey ON public.books USING btree (tenant_id);
-- Each index needs to include the tenant_id as a composite key
CREATE INDEX books_author_id_fkey ON public.books USING btree (tenant_id, author_id);

ALTER TABLE public.books ENABLE ROW LEVEL SECURITY;

-- Grant only select/insert/update/delete
-- may need to consider EXECUTE and USAGE later down the track
-- ALL shouldn't be used as Truncate is outside of RLS boundary see: https://www.postgresql.org/docs/9.5/ddl-rowsecurity.html
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE public.books TO tenancy_users;

-- policies need to be applied per table
CREATE POLICY tenant_book_isolation_policy
    ON public.books
    AS PERMISSIVE
    FOR ALL
    TO public -- apply this polcity to all roles/users
    USING ((tenant_id = (current_setting('app.current_tenant'::text))::uuid));