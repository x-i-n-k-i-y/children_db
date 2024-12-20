--
-- PostgreSQL database dump
--

-- Dumped from database version 17.2
-- Dumped by pg_dump version 17.2

-- Started on 2024-12-20 02:02:57

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
-- TOC entry 236 (class 1255 OID 16744)
-- Name: track_child_changes(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.track_child_changes() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    INSERT INTO child_history (child_id, group_id, date_from, date_to)
    VALUES (NEW.child_id, NEW.group_id, NOW(), NULL);
    RETURN NEW;
END;
$$;


ALTER FUNCTION public.track_child_changes() OWNER TO postgres;

--
-- TOC entry 235 (class 1255 OID 16742)
-- Name: track_group_changes(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.track_group_changes() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    INSERT INTO group_history (group_id, age_range, date_from, date_to)
    VALUES (NEW.group_id, NEW.age_range, NOW(), NULL);
    RETURN NEW;
END;
$$;


ALTER FUNCTION public.track_group_changes() OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 230 (class 1259 OID 16703)
-- Name: attendance; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.attendance (
    attendance_id integer NOT NULL,
    child_id integer NOT NULL,
    date date NOT NULL,
    was_present boolean NOT NULL
);


ALTER TABLE public.attendance OWNER TO postgres;

--
-- TOC entry 229 (class 1259 OID 16702)
-- Name: attendance_attendance_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.attendance_attendance_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.attendance_attendance_id_seq OWNER TO postgres;

--
-- TOC entry 4943 (class 0 OID 0)
-- Dependencies: 229
-- Name: attendance_attendance_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.attendance_attendance_id_seq OWNED BY public.attendance.attendance_id;


--
-- TOC entry 232 (class 1259 OID 16715)
-- Name: benefits; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.benefits (
    benefit_id integer NOT NULL,
    child_id integer NOT NULL,
    percentage numeric NOT NULL,
    date_from date NOT NULL,
    date_to date
);


ALTER TABLE public.benefits OWNER TO postgres;

--
-- TOC entry 231 (class 1259 OID 16714)
-- Name: benefits_benefit_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.benefits_benefit_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.benefits_benefit_id_seq OWNER TO postgres;

--
-- TOC entry 4944 (class 0 OID 0)
-- Dependencies: 231
-- Name: benefits_benefit_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.benefits_benefit_id_seq OWNED BY public.benefits.benefit_id;


--
-- TOC entry 226 (class 1259 OID 16674)
-- Name: child_history; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.child_history (
    history_id integer NOT NULL,
    child_id integer NOT NULL,
    group_id integer NOT NULL,
    date_from date NOT NULL,
    date_to date
);


ALTER TABLE public.child_history OWNER TO postgres;

--
-- TOC entry 225 (class 1259 OID 16673)
-- Name: child_history_history_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.child_history_history_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.child_history_history_id_seq OWNER TO postgres;

--
-- TOC entry 4945 (class 0 OID 0)
-- Dependencies: 225
-- Name: child_history_history_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.child_history_history_id_seq OWNED BY public.child_history.history_id;


--
-- TOC entry 224 (class 1259 OID 16662)
-- Name: children; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.children (
    child_id integer NOT NULL,
    first_name character varying(255) NOT NULL,
    group_id integer NOT NULL
);


ALTER TABLE public.children OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 16661)
-- Name: children_child_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.children_child_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.children_child_id_seq OWNER TO postgres;

--
-- TOC entry 4946 (class 0 OID 0)
-- Dependencies: 223
-- Name: children_child_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.children_child_id_seq OWNED BY public.children.child_id;


--
-- TOC entry 220 (class 1259 OID 16636)
-- Name: group_history; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.group_history (
    history_id integer NOT NULL,
    group_id integer NOT NULL,
    age_range character varying(10) NOT NULL,
    date_from date NOT NULL,
    date_to date
);


ALTER TABLE public.group_history OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 16635)
-- Name: group_history_history_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.group_history_history_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.group_history_history_id_seq OWNER TO postgres;

--
-- TOC entry 4947 (class 0 OID 0)
-- Dependencies: 219
-- Name: group_history_history_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.group_history_history_id_seq OWNED BY public.group_history.history_id;


--
-- TOC entry 218 (class 1259 OID 16629)
-- Name: group_tbl; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.group_tbl (
    group_id integer NOT NULL,
    group_name character varying(255) NOT NULL,
    age_range character varying(10) NOT NULL
);


ALTER TABLE public.group_tbl OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 16628)
-- Name: group_tbl_group_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.group_tbl_group_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.group_tbl_group_id_seq OWNER TO postgres;

--
-- TOC entry 4948 (class 0 OID 0)
-- Dependencies: 217
-- Name: group_tbl_group_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.group_tbl_group_id_seq OWNED BY public.group_tbl.group_id;


--
-- TOC entry 228 (class 1259 OID 16691)
-- Name: parents; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.parents (
    parent_id integer NOT NULL,
    child_id integer NOT NULL,
    full_name character varying(255) NOT NULL,
    date_from date NOT NULL,
    date_to date
);


ALTER TABLE public.parents OWNER TO postgres;

--
-- TOC entry 227 (class 1259 OID 16690)
-- Name: parents_parent_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.parents_parent_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.parents_parent_id_seq OWNER TO postgres;

--
-- TOC entry 4949 (class 0 OID 0)
-- Dependencies: 227
-- Name: parents_parent_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.parents_parent_id_seq OWNED BY public.parents.parent_id;


--
-- TOC entry 234 (class 1259 OID 16729)
-- Name: payments; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.payments (
    payment_id integer NOT NULL,
    child_id integer NOT NULL,
    month date NOT NULL,
    amount numeric NOT NULL,
    benefit numeric,
    balance numeric
);


ALTER TABLE public.payments OWNER TO postgres;

--
-- TOC entry 233 (class 1259 OID 16728)
-- Name: payments_payment_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.payments_payment_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.payments_payment_id_seq OWNER TO postgres;

--
-- TOC entry 4950 (class 0 OID 0)
-- Dependencies: 233
-- Name: payments_payment_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.payments_payment_id_seq OWNED BY public.payments.payment_id;


--
-- TOC entry 222 (class 1259 OID 16648)
-- Name: tariffs; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tariffs (
    tariff_id integer NOT NULL,
    group_id integer NOT NULL,
    date_from date NOT NULL,
    date_to date,
    rate numeric NOT NULL
);


ALTER TABLE public.tariffs OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 16647)
-- Name: tariffs_tariff_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tariffs_tariff_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tariffs_tariff_id_seq OWNER TO postgres;

--
-- TOC entry 4951 (class 0 OID 0)
-- Dependencies: 221
-- Name: tariffs_tariff_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tariffs_tariff_id_seq OWNED BY public.tariffs.tariff_id;


--
-- TOC entry 4743 (class 2604 OID 16706)
-- Name: attendance attendance_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.attendance ALTER COLUMN attendance_id SET DEFAULT nextval('public.attendance_attendance_id_seq'::regclass);


--
-- TOC entry 4744 (class 2604 OID 16718)
-- Name: benefits benefit_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.benefits ALTER COLUMN benefit_id SET DEFAULT nextval('public.benefits_benefit_id_seq'::regclass);


--
-- TOC entry 4741 (class 2604 OID 16677)
-- Name: child_history history_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.child_history ALTER COLUMN history_id SET DEFAULT nextval('public.child_history_history_id_seq'::regclass);


--
-- TOC entry 4740 (class 2604 OID 16665)
-- Name: children child_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.children ALTER COLUMN child_id SET DEFAULT nextval('public.children_child_id_seq'::regclass);


--
-- TOC entry 4738 (class 2604 OID 16639)
-- Name: group_history history_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.group_history ALTER COLUMN history_id SET DEFAULT nextval('public.group_history_history_id_seq'::regclass);


--
-- TOC entry 4737 (class 2604 OID 16632)
-- Name: group_tbl group_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.group_tbl ALTER COLUMN group_id SET DEFAULT nextval('public.group_tbl_group_id_seq'::regclass);


--
-- TOC entry 4742 (class 2604 OID 16694)
-- Name: parents parent_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.parents ALTER COLUMN parent_id SET DEFAULT nextval('public.parents_parent_id_seq'::regclass);


--
-- TOC entry 4745 (class 2604 OID 16732)
-- Name: payments payment_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.payments ALTER COLUMN payment_id SET DEFAULT nextval('public.payments_payment_id_seq'::regclass);


--
-- TOC entry 4739 (class 2604 OID 16651)
-- Name: tariffs tariff_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tariffs ALTER COLUMN tariff_id SET DEFAULT nextval('public.tariffs_tariff_id_seq'::regclass);


--
-- TOC entry 4933 (class 0 OID 16703)
-- Dependencies: 230
-- Data for Name: attendance; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.attendance (attendance_id, child_id, date, was_present) FROM stdin;
1	1	2023-01-01	t
2	1	2023-01-02	f
3	2	2023-01-01	t
4	2	2023-01-02	t
5	3	2023-01-01	t
6	3	2023-01-02	t
\.


--
-- TOC entry 4935 (class 0 OID 16715)
-- Dependencies: 232
-- Data for Name: benefits; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.benefits (benefit_id, child_id, percentage, date_from, date_to) FROM stdin;
1	1	50	2023-01-01	2023-06-30
2	2	30	2023-01-01	2023-12-31
3	3	100	2023-01-01	2023-12-31
\.


--
-- TOC entry 4929 (class 0 OID 16674)
-- Dependencies: 226
-- Data for Name: child_history; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.child_history (history_id, child_id, group_id, date_from, date_to) FROM stdin;
1	1	1	2023-01-01	2023-08-01
2	1	1	2023-08-02	\N
3	1	1	2023-01-01	2023-08-01
4	1	1	2023-08-02	\N
5	1	2	2023-01-01	\N
\.


--
-- TOC entry 4927 (class 0 OID 16662)
-- Dependencies: 224
-- Data for Name: children; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.children (child_id, first_name, group_id) FROM stdin;
1	Алиса	1
2	Борис	1
3	Алесь	1
4	Виктор	1
5	Мария	1
6	Сергей	1
7	Виктория	1
\.


--
-- TOC entry 4923 (class 0 OID 16636)
-- Dependencies: 220
-- Data for Name: group_history; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.group_history (history_id, group_id, age_range, date_from, date_to) FROM stdin;
1	1	2-3	2023-01-01	2023-08-01
2	1	4-5	2023-08-02	2023-08-20
\.


--
-- TOC entry 4921 (class 0 OID 16629)
-- Dependencies: 218
-- Data for Name: group_tbl; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.group_tbl (group_id, group_name, age_range) FROM stdin;
1	Ясли-1	2-3
2	Ясли-1	2-3
3	Ясли-1	2-3
4	Ясли-1	2-3
5	Средняя	4-5
6	Средняя	4-5
7	Средняя	4-5
8	Средняя	4-5
9	Старшая	6
10	Старшая	6
11	Старшая	6
12	Старшая	6
\.


--
-- TOC entry 4931 (class 0 OID 16691)
-- Dependencies: 228
-- Data for Name: parents; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.parents (parent_id, child_id, full_name, date_from, date_to) FROM stdin;
1	1	Иван Иванов	2023-01-01	2023-12-31
2	2	Мария Петрова	2023-01-01	2023-12-31
3	3	Сергей Викторов	2023-01-01	\N
\.


--
-- TOC entry 4937 (class 0 OID 16729)
-- Dependencies: 234
-- Data for Name: payments; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.payments (payment_id, child_id, month, amount, benefit, balance) FROM stdin;
1	1	2023-01-01	300	150	150
2	2	2023-01-01	350	105	245
3	3	2023-01-01	400	400	0
\.


--
-- TOC entry 4925 (class 0 OID 16648)
-- Dependencies: 222
-- Data for Name: tariffs; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.tariffs (tariff_id, group_id, date_from, date_to, rate) FROM stdin;
1	1	2023-01-01	2023-08-01	3.00
2	1	2023-08-02	2023-08-20	4.00
\.


--
-- TOC entry 4952 (class 0 OID 0)
-- Dependencies: 229
-- Name: attendance_attendance_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.attendance_attendance_id_seq', 6, true);


--
-- TOC entry 4953 (class 0 OID 0)
-- Dependencies: 231
-- Name: benefits_benefit_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.benefits_benefit_id_seq', 3, true);


--
-- TOC entry 4954 (class 0 OID 0)
-- Dependencies: 225
-- Name: child_history_history_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.child_history_history_id_seq', 5, true);


--
-- TOC entry 4955 (class 0 OID 0)
-- Dependencies: 223
-- Name: children_child_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.children_child_id_seq', 7, true);


--
-- TOC entry 4956 (class 0 OID 0)
-- Dependencies: 219
-- Name: group_history_history_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.group_history_history_id_seq', 2, true);


--
-- TOC entry 4957 (class 0 OID 0)
-- Dependencies: 217
-- Name: group_tbl_group_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.group_tbl_group_id_seq', 12, true);


--
-- TOC entry 4958 (class 0 OID 0)
-- Dependencies: 227
-- Name: parents_parent_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.parents_parent_id_seq', 3, true);


--
-- TOC entry 4959 (class 0 OID 0)
-- Dependencies: 233
-- Name: payments_payment_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.payments_payment_id_seq', 3, true);


--
-- TOC entry 4960 (class 0 OID 0)
-- Dependencies: 221
-- Name: tariffs_tariff_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.tariffs_tariff_id_seq', 2, true);


--
-- TOC entry 4759 (class 2606 OID 16708)
-- Name: attendance attendance_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.attendance
    ADD CONSTRAINT attendance_pkey PRIMARY KEY (attendance_id);


--
-- TOC entry 4761 (class 2606 OID 16722)
-- Name: benefits benefits_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.benefits
    ADD CONSTRAINT benefits_pkey PRIMARY KEY (benefit_id);


--
-- TOC entry 4755 (class 2606 OID 16679)
-- Name: child_history child_history_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.child_history
    ADD CONSTRAINT child_history_pkey PRIMARY KEY (history_id);


--
-- TOC entry 4753 (class 2606 OID 16667)
-- Name: children children_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.children
    ADD CONSTRAINT children_pkey PRIMARY KEY (child_id);


--
-- TOC entry 4749 (class 2606 OID 16641)
-- Name: group_history group_history_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.group_history
    ADD CONSTRAINT group_history_pkey PRIMARY KEY (history_id);


--
-- TOC entry 4747 (class 2606 OID 16634)
-- Name: group_tbl group_tbl_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.group_tbl
    ADD CONSTRAINT group_tbl_pkey PRIMARY KEY (group_id);


--
-- TOC entry 4757 (class 2606 OID 16696)
-- Name: parents parents_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.parents
    ADD CONSTRAINT parents_pkey PRIMARY KEY (parent_id);


--
-- TOC entry 4763 (class 2606 OID 16736)
-- Name: payments payments_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.payments
    ADD CONSTRAINT payments_pkey PRIMARY KEY (payment_id);


--
-- TOC entry 4751 (class 2606 OID 16655)
-- Name: tariffs tariffs_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tariffs
    ADD CONSTRAINT tariffs_pkey PRIMARY KEY (tariff_id);


--
-- TOC entry 4774 (class 2620 OID 16745)
-- Name: children child_changes; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER child_changes AFTER UPDATE ON public.children FOR EACH ROW EXECUTE FUNCTION public.track_child_changes();


--
-- TOC entry 4773 (class 2620 OID 16743)
-- Name: group_tbl group_changes; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER group_changes AFTER UPDATE ON public.group_tbl FOR EACH ROW EXECUTE FUNCTION public.track_group_changes();


--
-- TOC entry 4770 (class 2606 OID 16709)
-- Name: attendance attendance_child_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.attendance
    ADD CONSTRAINT attendance_child_id_fkey FOREIGN KEY (child_id) REFERENCES public.children(child_id);


--
-- TOC entry 4771 (class 2606 OID 16723)
-- Name: benefits benefits_child_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.benefits
    ADD CONSTRAINT benefits_child_id_fkey FOREIGN KEY (child_id) REFERENCES public.children(child_id);


--
-- TOC entry 4767 (class 2606 OID 16680)
-- Name: child_history child_history_child_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.child_history
    ADD CONSTRAINT child_history_child_id_fkey FOREIGN KEY (child_id) REFERENCES public.children(child_id);


--
-- TOC entry 4768 (class 2606 OID 16685)
-- Name: child_history child_history_group_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.child_history
    ADD CONSTRAINT child_history_group_id_fkey FOREIGN KEY (group_id) REFERENCES public.group_tbl(group_id);


--
-- TOC entry 4766 (class 2606 OID 16668)
-- Name: children children_group_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.children
    ADD CONSTRAINT children_group_id_fkey FOREIGN KEY (group_id) REFERENCES public.group_tbl(group_id);


--
-- TOC entry 4764 (class 2606 OID 16642)
-- Name: group_history group_history_group_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.group_history
    ADD CONSTRAINT group_history_group_id_fkey FOREIGN KEY (group_id) REFERENCES public.group_tbl(group_id);


--
-- TOC entry 4769 (class 2606 OID 16697)
-- Name: parents parents_child_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.parents
    ADD CONSTRAINT parents_child_id_fkey FOREIGN KEY (child_id) REFERENCES public.children(child_id);


--
-- TOC entry 4772 (class 2606 OID 16737)
-- Name: payments payments_child_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.payments
    ADD CONSTRAINT payments_child_id_fkey FOREIGN KEY (child_id) REFERENCES public.children(child_id);


--
-- TOC entry 4765 (class 2606 OID 16656)
-- Name: tariffs tariffs_group_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tariffs
    ADD CONSTRAINT tariffs_group_id_fkey FOREIGN KEY (group_id) REFERENCES public.group_tbl(group_id);


-- Completed on 2024-12-20 02:02:57

--
-- PostgreSQL database dump complete
--

