--liquibase formatted sql

--changeset Jordenn:create-table-calculation

CREATE TABLE calculation (
      id uuid NOT NULL,
      nanosystem_id uuid NOT NULL,
      object_id uuid NOT NULL,
      user_id uuid NOT NULL,
      q_vector_from DOUBLE PRECISION NOT NULL,
      q_vector_to DOUBLE PRECISION NOT NULL,
      q_space_method INT NOT NULL,
      q_scale_method INT NOT NULL,
      q_space_parameter DOUBLE PRECISION NOT NULL,
      phi_vector_from DOUBLE PRECISION NULL,
      phi_vector_to DOUBLE PRECISION NULL,
      phi_space_method INT NULL,
      phi_scale_method INT NULL,
      phi_space_parameter DOUBLE PRECISION NULL,
      theta_vector_from DOUBLE PRECISION NULL,
      theta_vector_to DOUBLE PRECISION NULL,
      theta_space_method INT NULL,
      theta_scale_method INT NULL,
      theta_space_parameter DOUBLE PRECISION NULL,
      input_date timestamp NOT NULL,
      calculate_start timestamp NOT NULL,
      calculate_end timestamp NOT NULL,
      PRIMARY KEY (id)
);
