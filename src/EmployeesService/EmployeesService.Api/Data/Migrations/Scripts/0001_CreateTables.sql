CREATE TABLE companies (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL
);

CREATE TABLE departments (
    id SERIAL PRIMARY KEY,
    company_id INT NOT NULL,
    name VARCHAR(200) NOT NULL,
    phone VARCHAR(50),
    CONSTRAINT fk_departments_companies FOREIGN KEY (company_id) 
        REFERENCES companies(id) ON DELETE CASCADE
);

CREATE TABLE employees (
    id SERIAL PRIMARY KEY,
    first_name VARCHAR(100) NOT NULL,
    surname VARCHAR(100) NOT NULL,
    phone VARCHAR(50),
    company_id INT NOT NULL,
    department_id INT NOT NULL,
    passport_type VARCHAR(50),
    passport_number VARCHAR(10),
    CONSTRAINT fk_employees_companies FOREIGN KEY (company_id) 
        REFERENCES companies(id) ON DELETE CASCADE,
    CONSTRAINT fk_employees_departments FOREIGN KEY (department_id)
        REFERENCES departments(id) ON DELETE CASCADE
);

CREATE INDEX idx_employees_company_id ON employees(company_id);
CREATE INDEX idx_employees_department_id ON employees(department_id);
CREATE INDEX idx_departments_company_id ON departments(company_id);