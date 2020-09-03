INSERT INTO public.tenants (
    tenant_id, created_at, name, status)
VALUES ('a5bab93a-1f7d-4fae-8bf6-ad7f2d6838fe', '2020-08-26 10:33:54.065784', 'First Tenant', null);

-- todo: refactor into a rule/trigger on insert to create sequence if it doesn't exist and nextval() automatically on insert
CREATE SEQUENCE IF NOT EXISTS public.a5bab93a1f7d4fae8bf6ad7f2d6838fe_books_book_id_seq;

GRANT USAGE, SELECT ON SEQUENCE a5bab93a1f7d4fae8bf6ad7f2d6838fe_books_book_id_seq TO tenancy_users;

INSERT INTO public.tenants (
     tenant_id, created_at, name, status)
VALUES ('e8124742-65da-485c-a977-0666bc337f5b', '2020-08-26 10:33:54.065784', 'Second Tenant', null);

CREATE SEQUENCE IF NOT EXISTS public.e812474265da485ca9770666bc337f5b_books_book_id_seq;
GRANT USAGE, SELECT ON SEQUENCE e812474265da485ca9770666bc337f5b_books_book_id_seq TO tenancy_users;

INSERT INTO public.authors (
	created_at, name, country)
	VALUES (current_timestamp, 'Phillip K Dick', 'USA');
	
INSERT INTO public.authors (
	created_at, name, country)
	VALUES (current_timestamp, 'Alfred Bester', 'USA');

COPY 
    genres(name) 
FROM 
    '/tmp/genres.txt'
USING 
    DELIMITERS '|' WITH NULL AS '\null';

INSERT INTO public.books (
    book_id, book_number, created_at, title, year, author_id, tenant_id, genre_id)
VALUES ('eea7dda3-dbd1-45cd-8bb2-483055b9904d', nextval('a5bab93a1f7d4fae8bf6ad7f2d6838fe_books_book_id_seq'), '2020-08-26 10:33:54.065784', 'MyFirstBook',  '2020-08-26 10:33:54.065784', 1, 'a5bab93a-1f7d-4fae-8bf6-ad7f2d6838fe', (select genre_id from genres where name = 'Science fiction') );

INSERT INTO public.books (
    book_id, book_number, created_at, title, year, author_id, tenant_id, genre_id)
VALUES ('81049067-9541-4ad1-84a6-3e7112c08728', nextval('e812474265da485ca9770666bc337f5b_books_book_id_seq'), '2020-08-26 10:33:54.065784', 'MySecondBook', '2020-08-26 10:33:54.065784', 1, 'e8124742-65da-485c-a977-0666bc337f5b', (select genre_id from genres where name = 'Science fiction'));