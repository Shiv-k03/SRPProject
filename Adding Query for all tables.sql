SET IDENTITY_INSERT Departments ON;

INSERT INTO Departments
(DepartmentId, DepartmentName, DepartmentCode, DepartmentType, Description,
 HeadOfDepartmentId, CreatedAt, UpdatedAt, IsDeleted)
VALUES
(1, 'Arts',        'ART', 1, 'Arts Department',        NULL, GETUTCDATE(), GETUTCDATE(), 0),
(2, 'Commerce',    'COM', 2, 'Commerce Department',    NULL, GETUTCDATE(), GETUTCDATE(), 0),
(3, 'Science',     'SCI', 3, 'Science Department',     NULL, GETUTCDATE(), GETUTCDATE(), 0),
(4, 'Engineering', 'ENG', 4, 'Engineering Department', NULL, GETUTCDATE(), GETUTCDATE(), 0),
(5, 'Medical',     'MED', 5, 'Medical Department',     NULL, GETUTCDATE(), GETUTCDATE(), 0),
(6, 'Management',  'MGM', 6, 'Management Department',  NULL, GETUTCDATE(), GETUTCDATE(), 0);

SET IDENTITY_INSERT Departments OFF;