﻿

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

-- Table: public.tenants
CREATE TABLE public.tenants (
    tenant_id uuid PRIMARY KEY,
    created_at timestamptz NOT NULL,
    name text,
    status text
);    

ALTER TABLE public.tenants
    OWNER to postgres;

ALTER TABLE public.tenants
    ENABLE ROW LEVEL SECURITY;

GRANT SELECT ON TABLE public.tenants TO tenancy_users;

-- POLICY: tenant_isolation_policy

CREATE POLICY tenant_isolation_policy
    ON public.tenants
    AS PERMISSIVE
    FOR ALL
    TO public
    USING ((tenant_id = (current_setting('app.current_tenant'::text))::uuid));

-- DROP POLICY tenant_isolation_policy ON public.tenants;

CREATE TABLE public.authors (
    author_id uuid PRIMARY KEY,
    created_at timestamptz NOT NULL,
    name text,
    country text    
);

ALTER TABLE public.authors
    OWNER to postgres;

-- Grant only select/insert/update/delete
-- may need to consider EXECUTE and USAGE later down the track
-- ALL shouldn't be used as Truncate is outside of RLS boundary, and ALTER TABLE can disable RLS, see: https://www.postgresql.org/docs/9.5/ddl-rowsecurity.html
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE public.authors TO tenancy_users;

CREATE TABLE public.books (
    book_id uuid PRIMARY KEY,
    created_at timestamptz NOT NULL,
    title text,
    year timestamptz NOT NULL,
    author_id uuid REFERENCES public.authors(author_id),
    tenant_id uuid REFERENCES public.tenants(tenant_id)  
);

ALTER TABLE public.books
    OWNER to postgres;

ALTER TABLE public.books
    ENABLE ROW LEVEL SECURITY;

-- Grant only select/insert/update/delete
-- may need to consider EXECUTE and USAGE later down the track
-- ALL shouldn't be used as Truncate is outside of RLS boundary see: https://www.postgresql.org/docs/9.5/ddl-rowsecurity.html
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE public.books TO tenancy_users;

-- Index: IX_books_AuthorId
-- DROP INDEX public."IX_books_AuthorId";

CREATE INDEX "IX_books_AuthorId"
    ON public.books USING btree
        (author_id ASC NULLS LAST);

-- Index: IX_books_TenantId
-- DROP INDEX public."IX_books_TenantId";

CREATE INDEX "IX_books_TenantId"
    ON public.books USING btree
        (tenant_id ASC NULLS LAST);
-- POLICY: tenant_book_isolation_policy

-- DROP POLICY tenant_book_isolation_policy ON public.books;


-- policies need to be applied per table

CREATE POLICY tenant_book_isolation_policy
    ON public.books
    AS PERMISSIVE
    FOR ALL
    TO public -- apply this polcity to all roles/users
    USING ((tenant_id = (current_setting('app.current_tenant'::text))::uuid));