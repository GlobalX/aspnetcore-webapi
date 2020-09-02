

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
CREATE TABLE public.tenants
(
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "Name" text COLLATE pg_catalog."default",
    "Status" text COLLATE pg_catalog."default",
    CONSTRAINT "PK_tenants" PRIMARY KEY ("Id")
)

    TABLESPACE pg_default;

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
    USING (("Id" = (current_setting('app.current_tenant'::text))::uuid));

-- DROP POLICY tenant_isolation_policy ON public.tenants;

CREATE TABLE public.authors
(
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "Name" text COLLATE pg_catalog."default",
    "Country" text COLLATE pg_catalog."default",
    CONSTRAINT "PK_authors" PRIMARY KEY ("Id")
)

    TABLESPACE pg_default;

ALTER TABLE public.authors
    OWNER to postgres;

-- Grant only select/insert/update/delete
-- may need to consider EXECUTE and USAGE later down the track
-- ALL shouldn't be used as Truncate is outside of RLS boundary, and ALTER TABLE can disable RLS, see: https://www.postgresql.org/docs/9.5/ddl-rowsecurity.html
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE public.authors TO tenancy_users;

-- Table: public.books

-- DROP TABLE public.books;

CREATE TABLE public.books
(
    "Id" uuid NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "Title" text COLLATE pg_catalog."default",
    "Year" timestamp without time zone NOT NULL,
    "AuthorId" uuid,
    "TenantId" uuid,
    CONSTRAINT "PK_books" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_books_authors_AuthorId" FOREIGN KEY ("AuthorId")
        REFERENCES public.authors ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE RESTRICT,
    CONSTRAINT "FK_books_tenants_TenantId" FOREIGN KEY ("TenantId")
        REFERENCES public.tenants ("Id") MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE RESTRICT
)

    TABLESPACE pg_default;

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
        ("AuthorId" ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: IX_books_TenantId

-- DROP INDEX public."IX_books_TenantId";

CREATE INDEX "IX_books_TenantId"
    ON public.books USING btree
        ("TenantId" ASC NULLS LAST)
    TABLESPACE pg_default;
-- POLICY: tenant_book_isolation_policy

-- DROP POLICY tenant_book_isolation_policy ON public.books;


-- policies need to be applied per table

CREATE POLICY tenant_book_isolation_policy
    ON public.books
    AS PERMISSIVE
    FOR ALL
    TO public -- apply this polcity to all roles/users
    USING (("TenantId" = (current_setting('app.current_tenant'::text))::uuid));