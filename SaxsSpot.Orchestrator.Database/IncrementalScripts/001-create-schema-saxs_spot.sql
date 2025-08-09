--liquibase formatted sql

--changeset Jordenn:create-schema-saxs_spot
CREATE SCHEMA IF NOT EXISTS saxs;
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
