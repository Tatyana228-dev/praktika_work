--
-- PostgreSQL database dump
--

\restrict QaDKiXrGwumXg5101Bn5qgIv1i765BOYBo4tq9HeGkswdU2s7FPBALyXssX6j8K

-- Dumped from database version 17.6
-- Dumped by pg_dump version 17.6

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: app; Type: SCHEMA; Schema: -; Owner: app
--

CREATE SCHEMA app;


ALTER SCHEMA app OWNER TO app;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: discount; Type: TABLE; Schema: app; Owner: app
--

CREATE TABLE app.discount (
    id integer NOT NULL,
    partnerid integer,
    percentage double precision
);


ALTER TABLE app.discount OWNER TO app;

--
-- Name: discount_id_seq; Type: SEQUENCE; Schema: app; Owner: app
--

CREATE SEQUENCE app.discount_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE app.discount_id_seq OWNER TO app;

--
-- Name: discount_id_seq; Type: SEQUENCE OWNED BY; Schema: app; Owner: app
--

ALTER SEQUENCE app.discount_id_seq OWNED BY app.discount.id;


--
-- Name: partner; Type: TABLE; Schema: app; Owner: app
--

CREATE TABLE app.partner (
    id integer NOT NULL,
    type text NOT NULL,
    name text NOT NULL,
    director text NOT NULL,
    email text NOT NULL,
    phone text NOT NULL,
    legaladdress text NOT NULL,
    rating integer
);


ALTER TABLE app.partner OWNER TO app;

--
-- Name: partner_id_seq; Type: SEQUENCE; Schema: app; Owner: app
--

CREATE SEQUENCE app.partner_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE app.partner_id_seq OWNER TO app;

--
-- Name: partner_id_seq; Type: SEQUENCE OWNED BY; Schema: app; Owner: app
--

ALTER SEQUENCE app.partner_id_seq OWNED BY app.partner.id;


--
-- Name: sale; Type: TABLE; Schema: app; Owner: app
--

CREATE TABLE app.sale (
    id integer NOT NULL,
    partnerid integer,
    quantity integer NOT NULL,
    date date NOT NULL,
    product_name character varying NOT NULL
);


ALTER TABLE app.sale OWNER TO app;

--
-- Name: sale_id_seq; Type: SEQUENCE; Schema: app; Owner: app
--

CREATE SEQUENCE app.sale_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE app.sale_id_seq OWNER TO app;

--
-- Name: sale_id_seq; Type: SEQUENCE OWNED BY; Schema: app; Owner: app
--

ALTER SEQUENCE app.sale_id_seq OWNED BY app.sale.id;


--
-- Name: discount id; Type: DEFAULT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.discount ALTER COLUMN id SET DEFAULT nextval('app.discount_id_seq'::regclass);


--
-- Name: partner id; Type: DEFAULT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.partner ALTER COLUMN id SET DEFAULT nextval('app.partner_id_seq'::regclass);


--
-- Name: sale id; Type: DEFAULT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.sale ALTER COLUMN id SET DEFAULT nextval('app.sale_id_seq'::regclass);


--
-- Data for Name: discount; Type: TABLE DATA; Schema: app; Owner: app
--

COPY app.discount (id, partnerid, percentage) FROM stdin;
3	\N	5
9	\N	0
8	\N	5
30	\N	0
31	61	5
\.


--
-- Data for Name: partner; Type: TABLE DATA; Schema: app; Owner: app
--

COPY app.partner (id, type, name, director, email, phone, legaladdress, rating) FROM stdin;
61	ПАО	Проверка	Паршаков.Д.В	dirpr@yandex.ru	+79423332211	Г.Пермь ул.Ломоносова 67	8
\.


--
-- Data for Name: sale; Type: TABLE DATA; Schema: app; Owner: app
--

COPY app.sale (id, partnerid, quantity, date, product_name) FROM stdin;
33	61	12000	2026-03-13	Продукт1
\.


--
-- Name: discount_id_seq; Type: SEQUENCE SET; Schema: app; Owner: app
--

SELECT pg_catalog.setval('app.discount_id_seq', 31, true);


--
-- Name: partner_id_seq; Type: SEQUENCE SET; Schema: app; Owner: app
--

SELECT pg_catalog.setval('app.partner_id_seq', 61, true);


--
-- Name: sale_id_seq; Type: SEQUENCE SET; Schema: app; Owner: app
--

SELECT pg_catalog.setval('app.sale_id_seq', 34, true);


--
-- Name: discount discount_pkey; Type: CONSTRAINT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.discount
    ADD CONSTRAINT discount_pkey PRIMARY KEY (id);


--
-- Name: partner partner_pkey; Type: CONSTRAINT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.partner
    ADD CONSTRAINT partner_pkey PRIMARY KEY (id);


--
-- Name: sale sale_pkey; Type: CONSTRAINT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.sale
    ADD CONSTRAINT sale_pkey PRIMARY KEY (id);


--
-- Name: discount discount_partnerid_fkey; Type: FK CONSTRAINT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.discount
    ADD CONSTRAINT discount_partnerid_fkey FOREIGN KEY (partnerid) REFERENCES app.partner(id) ON DELETE SET NULL;


--
-- Name: sale sale_partnerid_fkey; Type: FK CONSTRAINT; Schema: app; Owner: app
--

ALTER TABLE ONLY app.sale
    ADD CONSTRAINT sale_partnerid_fkey FOREIGN KEY (partnerid) REFERENCES app.partner(id) ON DELETE SET NULL;


--
-- PostgreSQL database dump complete
--

\unrestrict QaDKiXrGwumXg5101Bn5qgIv1i765BOYBo4tq9HeGkswdU2s7FPBALyXssX6j8K

