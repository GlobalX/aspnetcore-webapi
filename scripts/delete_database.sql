-- see active connections
select * from  pg_stat_activity 

-- kill any active connections one by one on the db
SELECT pg_terminate_backend(<pid>)
FROM pg_stat_activity
WHERE pg_stat_activity.datname = 'tenancytest' -- ← change this to your DB
  AND pid <> pg_backend_pid();
  
-- from the postgres database, issue:
drop database tenancytest