-- Consultas
SELECT * FROM usuarios;
SELECT * FROM empleados;
-- Consulta para ver los datos de ambas tablas
SELECT u.userId, u.Login, u.Nombre, u.Paterno, u.Materno, e.Sueldo, e.FechaIngreso FROM usuarios u JOIN empleados e ON u.userId = e.userId;
-- Consulta para depurar los ID diferentes de 6,7,9,10 en usuarios
DELETE FROM usuarios WHERE userId NOT IN (6, 7, 9, 10);
-- Actualizar el dato de sueldo en un 10 porciento a los empleados que tienen fechas entre el 2000 y 2001
UPDATE empleados SET Sueldo = Sueldo * 1.10 WHERE YEAR(FechaIngreso) BETWEEN 2000 AND 2001;
-- Consulta para traer el nombre de usuario y fecha de ingreso de los usuarios que ganen más de 10000 y su apellido comience con T ordenado del más reciente al más antiguo
SELECT u.Nombre, u.Paterno, u.Materno, e.FechaIngreso FROM usuarios u JOIN empleados e ON u.userId = e.userId
WHERE 
    e.Sueldo > 10000 
    AND u.Paterno LIKE 'T%'
ORDER BY 
    e.FechaIngreso DESC;
-- Consulta donde agrupes a los empleados por sueldo, un grupo con los que ganan menos de 1200 y uno mayor o igual a 1200, ¿cuántos hay en cada grupo?
SELECT 
    CASE 
        WHEN Sueldo < 12000 THEN 'Menos de 12000'
        ELSE '12000 o más'
    END AS GrupoSueldo,
    COUNT(*) AS Cantidad
FROM 
    empleados
GROUP BY 
    GrupoSueldo;
-- Mostrar a los de 12000 o mas
SELECT u.Nombre, e.Sueldo FROM empleados e
JOIN 
    usuarios u ON e.userId = u.userId
WHERE 
    e.Sueldo >= 12000
ORDER BY 
    e.Sueldo DESC, 
    u.Nombre;
-- Mostrar a los de menos de 12000 
SELECT u.Nombre, e.Sueldo FROM empleados e
JOIN 
    usuarios u ON e.userId = u.userId
WHERE 
    e.Sueldo <= 12000
ORDER BY 
    e.Sueldo DESC, 
    u.Nombre;