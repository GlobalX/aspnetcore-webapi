CREATE SCHEMA trust_views;
GRANT USAGE ON SCHEMA trust_views TO appuser;
GRANT SELECT, INSERT, UPDATE ON ALL TABLES IN SCHEMA trust_views TO appuser;
-- CREATE LOCAL VIEWS
CREATE VIEW trust_views.books_vw AS SELECT *
                                      FROM public.books
                                      WHERE "TenantId" = current_setting('app.current_tenant')::uuid
WITH CHECK OPTION;
