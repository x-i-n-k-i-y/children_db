--список детей на дату
SELECT
    c.child_id,
    c.first_name,
    p.full_name AS parent_name,
    g.group_name,
    gh.age_range,
    t.rate AS current_rate,
    COALESCE(b.percentage, 0) AS benefit_percentage,
    a.date AS attendance_date,
    a.was_present
FROM
    children c
JOIN
    parents p ON c.child_id = p.child_id
JOIN
    child_history ch ON c.child_id = ch.child_id
JOIN
    group_history gh ON ch.group_id = gh.group_id
JOIN
    group_tbl g ON gh.group_id = g.group_id
JOIN
    tariffs t ON gh.group_id = t.group_id
LEFT JOIN
    benefits b ON c.child_id = b.child_id AND '2023-12-01' BETWEEN b.date_from AND COALESCE(b.date_to, '9999-12-31')
LEFT JOIN
    attendance a ON c.child_id = a.child_id AND a.date = '2023-12-01'
WHERE
    '2023-01-01' BETWEEN gh.date_from AND COALESCE(gh.date_to, '9999-12-31')
    AND '2023-01-01' BETWEEN t.date_from AND COALESCE(t.date_to, '9999-12-31')
    AND '2023-01-01' BETWEEN p.date_from AND COALESCE(p.date_to, '9999-12-31')
GROUP BY
    c.child_id, c.first_name, p.full_name, g.group_name, gh.age_range, t.rate, b.percentage, a.date, a.was_present;


--табель по группе
SELECT
    a.date AS attendance_date,
    c.child_id,
    c.first_name,
    g.group_name,
    a.was_present
FROM
    attendance a
JOIN
    children c ON a.child_id = c.child_id
JOIN
    group_tbl g ON c.group_id = g.group_id
WHERE
    a.date BETWEEN '2023-01-01' AND '2023-12-31'
ORDER BY
    a.date, c.child_id;


--табель по саду
SELECT
    g.group_name,
    gh.age_range,
    COUNT(DISTINCT c.child_id) AS children_count,
    COUNT(a.attendance_id) FILTER (WHERE a.was_present) AS attendance_count,
    a.date AS attendance_date
FROM
    group_tbl g
JOIN
    group_history gh ON g.group_id = gh.group_id
JOIN
    children c ON c.group_id = g.group_id
JOIN
    attendance a ON c.child_id = a.child_id
WHERE
    a.date BETWEEN '2023-01-01' AND '2023-12-31'
    AND '2023-01-01' BETWEEN gh.date_from AND COALESCE(gh.date_to, '9999-12-31')
GROUP BY
    g.group_name, gh.age_range, a.date
ORDER BY
    a.date, g.group_name;



--список детей сгруппированных в определенный месяц

SELECT
    g.group_name,
    gh.age_range,
    c.child_id,
    c.first_name
FROM
    children c
JOIN
    group_tbl g ON c.group_id = g.group_id
JOIN
    group_history gh ON c.group_id = gh.group_id
WHERE
    '2023-01-01' BETWEEN gh.date_from AND COALESCE(gh.date_to, '2023-01-31')
ORDER BY
    g.group_name, c.first_name;

--список групп на определенную дату

SELECT
    g.group_id,
    g.group_name,
    gh.age_range,
    t.rate AS current_rate
FROM
    group_tbl g
JOIN
    group_history gh ON g.group_id = gh.group_id
LEFT JOIN
    tariffs t ON gh.group_id = t.group_id
WHERE
    '2023-01-01' BETWEEN gh.date_from AND COALESCE(gh.date_to, '9999-12-31')
    AND '2023-01-01' BETWEEN t.date_from AND COALESCE(t.date_to, '9999-12-31');

