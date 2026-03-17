-- ============================================
-- SEED DATA: Users Table
-- ============================================
INSERT INTO [FeatureFlagDB].[dbo].[Users] 
(FirstName, LastName, Email, UserRole, DOB, Region)
VALUES
('John', 'Doe', 'john.doe@company.com', 'Admin', '1990-05-15', 'North America'),
('Sarah', 'Connor', 'sarah.connor@company.com', 'Admin', '1985-08-22', 'North America'),

('Jane', 'Smith', 'jane.smith@company.com', 'Manager', '1988-03-10', 'Europe'),
('Michael', 'Johnson', 'michael.johnson@company.com', 'Manager', '1992-11-05', 'Europe'),

('David', 'Williams', 'david.williams@company.com', 'Employee', '1995-07-18', 'Asia'),
('Emily', 'Brown', 'emily.brown@company.com', 'Employee', '1993-02-28', 'Asia'),

('Bob', 'Wilson', 'bob.wilson@company.com', 'Tester', '1991-09-12', 'North America'),
('Alice', 'Davis', 'alice.davis@company.com', 'Tester', '1994-04-20', 'North America'),

('Chris', 'Martinez', 'chris.martinez@company.com', 'Employee', '1989-12-01', 'Europe'),

('Lisa', 'Anderson', 'lisa.anderson@company.com', 'Employee', '1996-06-15', 'Asia');

SELECT Id, FirstName, LastName, Email, UserRole, Region 
FROM [FeatureFlagDB].[dbo].[Users] 
ORDER BY Id;




-- ============================================
-- SEED DATA: Features Table
-- ============================================

INSERT INTO [FeatureFlagDB].[dbo].[Features] 
(FeatureName, FeatureDesc, FeatureCode)
VALUES
('Delete User', 'Permission to delete user accounts from the system', 'FLT_AUTH_001'),
('User Edit', 'Permission to edit user profile information', 'FLT_AUTH_002'),
('Feature List', 'Permission to view feature permissions list', 'FLT_AUTH_003'),

('Dark Mode', 'Toggle dark/light UI theme for the application', 'FLT_UI_002'),

('Data Export', 'Export data to CSV/Excel formats', 'FLT_DATA_003'),

('Email Notifications', 'Receive email notifications for system events', 'FLT_NOT_005');

SELECT Id, FeatureName, FeatureCode 
FROM [FeatureFlagDB].[dbo].[Features] 
ORDER BY Id;



-- ============================================
-- SEED DATA: FeaturePermissions Table
-- ============================================

INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (1, 'GLOBAL', '1', 0);

INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (1, 'ROLE', 'Admin', 1);

INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (1, 'USER', '1', 0);  


INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (2, 'GLOBAL', '1', 1);

INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (2, 'ROLE', 'Tester', 0);

INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (2, 'COUNTRY', 'India', 1);

INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (2, 'USER', '3', 1);  


INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (3, 'GLOBAL', '1', 0);

INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (3, 'ROLE', 'Admin', 1);

INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (3, 'ROLE', 'Manager', 1);

INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (3, 'COUNTRY', 'North America', 1);

INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (3, 'COUNTRY', 'Europe', 1);


INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (4, 'GLOBAL', '1', 1);

INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (4, 'ROLE', 'Tester', 0);

INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (4, 'USER', '7', 0); 


INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (5, 'GLOBAL', '1', 1);

INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (5, 'ROLE', 'Tester', 0);

INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (5, 'USER', '1', 0);  


INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (6, 'GLOBAL', '1', 0);

INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (6, 'ROLE', 'Admin', 1);

INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (6, 'COUNTRY', 'Europe', 1);

INSERT INTO [FeatureFlagDB].[dbo].[FeaturePermissions] 
(FeatureId, AccessLevel, AccessId, Val)
VALUES (6, 'USER', '5', 1);  


SELECT 
    f.FeatureName,
    fp.AccessLevel,
    fp.AccessId,
    CASE WHEN fp.Val = 1 THEN '✅ Granted' ELSE '❌ Denied' END AS Permission
FROM [FeatureFlagDB].[dbo].[FeaturePermissions] fp
JOIN [FeatureFlagDB].[dbo].[Features] f ON fp.FeatureId = f.Id
ORDER BY f.Id, 
    CASE fp.AccessLevel 
        WHEN 'USER' THEN 1 
        WHEN 'ROLE' THEN 2 
        WHEN 'COUNTRY' THEN 3 
        WHEN 'GLOBAL' THEN 4 
    END;