FROM liquibase/liquibase:latest

COPY ./IncrementalScripts /liquibase/IncrementalScripts
COPY liquibase.properties /liquibase/liquibase.properties
COPY config.xml /liquibase/config.xml

USER root

ADD https://jdbc.postgresql.org/download/postgresql-42.2.20.jar /liquibase/lib/postgresql-42.2.20.jar

ENTRYPOINT ["liquibase", "update"]