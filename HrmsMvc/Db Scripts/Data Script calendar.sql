SET IDENTITY_INSERT [dbo].[calendar_event_employees] ON 

INSERT [dbo].[calendar_event_employees] ([Id], [task_id], [employee_id]) VALUES (1, 1, 8)
INSERT [dbo].[calendar_event_employees] ([Id], [task_id], [employee_id]) VALUES (2, 2, 3066)
INSERT [dbo].[calendar_event_employees] ([Id], [task_id], [employee_id]) VALUES (9, 7, 8)
INSERT [dbo].[calendar_event_employees] ([Id], [task_id], [employee_id]) VALUES (23, 6, 8)
INSERT [dbo].[calendar_event_employees] ([Id], [task_id], [employee_id]) VALUES (24, 8, 8)
SET IDENTITY_INSERT [dbo].[calendar_event_employees] OFF
SET IDENTITY_INSERT [dbo].[calendar_event_info] ON 

INSERT [dbo].[calendar_event_info] ([Id], [heading], [note], [status], [event_type]) VALUES (1, N'ef', N'sdf', 1, 1)
INSERT [dbo].[calendar_event_info] ([Id], [heading], [note], [status], [event_type]) VALUES (2, N'dsf', N'sdf', 1, 1)
INSERT [dbo].[calendar_event_info] ([Id], [heading], [note], [status], [event_type]) VALUES (6, N'Applied for leave', N'', 1, 3)
INSERT [dbo].[calendar_event_info] ([Id], [heading], [note], [status], [event_type]) VALUES (7, N'Applied for leave', N'', 1, 3)
INSERT [dbo].[calendar_event_info] ([Id], [heading], [note], [status], [event_type]) VALUES (8, N'Applied for leave', N'', 1, 3)
SET IDENTITY_INSERT [dbo].[calendar_event_info] OFF
SET IDENTITY_INSERT [dbo].[calendar_event_info_dates] ON 

INSERT [dbo].[calendar_event_info_dates] ([Id], [task_id], [start_date], [end_date]) VALUES (1, 1, N'2018-03-01 09:00:00', N'2018-03-03 11:00:00')
INSERT [dbo].[calendar_event_info_dates] ([Id], [task_id], [start_date], [end_date]) VALUES (2, 2, N'2018-03-01 08:00:00', N'2018-03-02 09:00:00')
INSERT [dbo].[calendar_event_info_dates] ([Id], [task_id], [start_date], [end_date]) VALUES (6, 6, N'2018-03-08 09:00:00', N'2018-03-08 13:00:00')
INSERT [dbo].[calendar_event_info_dates] ([Id], [task_id], [start_date], [end_date]) VALUES (7, 7, N'2018-03-15 09:00:00', N'2018-03-16 18:00:00')
INSERT [dbo].[calendar_event_info_dates] ([Id], [task_id], [start_date], [end_date]) VALUES (8, 8, N'2018-03-22 09:00:00', N'2018-03-22 18:00:00')
SET IDENTITY_INSERT [dbo].[calendar_event_info_dates] OFF
SET IDENTITY_INSERT [dbo].[calendar_event_log] ON 

INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (1, 1, 7, N'2018-03-02 18:32:50', N'Task created and assigned to employees by admin.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (2, 2, 7, N'2018-03-02 20:09:38', N'Task created and assigned to employees by admin.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (6, 6, 8, N'2018-03-03 10:54:29', N'Leave added.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (7, 7, 8, N'2018-03-03 11:05:11', N'Leave added.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (8, 8, 8, N'2018-03-03 11:09:01', N'Leave added.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (9, 0, 8, N'2018-03-03 11:29:58', N'Leave added.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (10, 0, 8, N'2018-03-03 11:57:26', N'Leave added.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (11, 0, 8, N'2018-03-03 11:57:39', N'Leave added.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (12, 0, 8, N'2018-03-03 12:01:15', N'Leave added.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (13, 0, 8, N'2018-03-03 12:01:23', N'Leave added.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (14, 0, 8, N'2018-03-03 12:03:39', N'Leave added.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (15, 0, 8, N'2018-03-03 12:03:47', N'Leave added.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (16, 0, 8, N'2018-03-03 14:39:24', N'Leave added.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (17, 0, 8, N'2018-03-03 14:39:33', N'Leave added.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (18, 0, 8, N'2018-03-03 17:25:44', N'Leave added.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (19, 0, 8, N'2018-03-03 17:27:50', N'Leave added.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (20, 0, 8, N'2018-03-03 17:29:28', N'Leave added.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (21, 0, 8, N'2018-03-03 17:30:57', N'Leave added.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (22, 0, 8, N'2018-03-03 17:31:13', N'Leave added.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (23, 0, 8, N'2018-03-03 17:31:19', N'Leave added.')
INSERT [dbo].[calendar_event_log] ([Id], [task_id], [created_by], [created_at], [event_log]) VALUES (24, 0, 8, N'2018-03-03 17:50:43', N'Leave added.')
SET IDENTITY_INSERT [dbo].[calendar_event_log] OFF