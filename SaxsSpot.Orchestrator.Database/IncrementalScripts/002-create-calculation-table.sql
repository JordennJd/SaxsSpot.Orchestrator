--liquibase formatted sql

--changeset Jordenn:create-table-calculation

CREATE TABLE calculation (
      id uuid NOT NULL,
      nanosystem_id uuid NOT NULL,
      object_id uuid NOT NULL,
      user_id uuid NOT NULL,
      q_vector_from FLOAT NOT NULL,
      q_vector_to FLOAT NOT NULL,
      q_space_method INT NOT NULL,
      q_space_parameter FLOAT NOT NULL,
      phi_vector_from FLOAT NULL,
      phi_vector_to FLOAT NULL,
      phi_space_method INT NULL,
      phi_space_parameter FLOAT NULL,
      theta_vector_from FLOAT NULL,
      theta_vector_to FLOAT NULL,
      theta_space_method INT NULL,
      theta_space_parameter FLOAT NULL,
      input_date DATE NOT NULL,
      calculate_start DATE NOT NULL,
      calculate_end DATE NOT NULL,
      PRIMARY KEY (id)
);
