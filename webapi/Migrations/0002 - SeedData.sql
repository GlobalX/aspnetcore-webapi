INSERT INTO public.tenants(
    "Id", "CreatedAt", "Name", "Status")
VALUES ('a5bab93a-1f7d-4fae-8bf6-ad7f2d6838fe', '2020-08-26 10:33:54.065784', 'FirstTenant', null);

INSERT INTO public.tenants(
    "Id", "CreatedAt", "Name", "Status")
VALUES ('e8124742-65da-485c-a977-0666bc337f5b', '2020-08-26 10:33:54.065784', 'FirstTenant', null);

INSERT INTO public.books(
    "Id", "CreatedAt", "Title", "Year", "AuthorId", "TenantId")
VALUES ('eea7dda3-dbd1-45cd-8bb2-483055b9904d',  '2020-08-26 10:33:54.065784', 'MyFirstBook',  '2020-08-26 10:33:54.065784', null, 'a5bab93a-1f7d-4fae-8bf6-ad7f2d6838fe');

INSERT INTO public.books(
    "Id", "CreatedAt", "Title", "Year", "AuthorId", "TenantId")
VALUES ('81049067-9541-4ad1-84a6-3e7112c08728',  '2020-08-26 10:33:54.065784', 'MySecondBook', '2020-08-26 10:33:54.065784', null, 'e8124742-65da-485c-a977-0666bc337f5b');