-- Table: public.tenants

-- DROP TABLE public.tenants;

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

GRANT ALL ON TABLE public.tenants TO appuser;

GRANT ALL ON TABLE public.tenants TO postgres;
-- POLICY: tenant_isolation_policy

-- DROP POLICY tenant_isolation_policy ON public.tenants;

CREATE POLICY tenant_isolation_policy
    ON public.tenants
    AS PERMISSIVE
    FOR ALL
    TO public
    USING (("Id" = (current_setting('app.current_tenant'::text))::uuid));

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

GRANT ALL ON TABLE public.authors TO appuser;

GRANT ALL ON TABLE public.authors TO postgres;

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

GRANT ALL ON TABLE public.books TO appuser;

GRANT ALL ON TABLE public.books TO postgres;
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

CREATE POLICY tenant_book_isolation_policy
    ON public.books
    AS PERMISSIVE
    FOR ALL
    TO public
    USING (("TenantId" = (current_setting('app.current_tenant'::text))::uuid));