export class AppConstants {
    public static get baseURL(): string { return "http://localhost:4200/api"; }
};
export enum UserPrivilages {
    admin = 1,
    user = 2,
    superuser = 3
};
export enum Designation {
    HR = 1,
    SOFTWARE_ENGINEER = 2,
    SENIOR_SOFTWARE_ENGINEER = 3,
    QA_ENGINEER = 4,
    SENIOR_QA_ENGINEER = 5,
    ASSOCIATE_SOFTWARE_ENGINEER = 6,
    BUSINESS_ANALYST = 7,
    SENIOR_BUSINESS_ANALYST = 8,
    ARCHITECT = 9,
    DIRECTOR = 10
};
export enum CalendarEventType {
    Task = 1, Birthday = 2, Leave = 3, Other = 4
};
export enum CalendarEventStatus {
    ASSIGNED = 1, INPROGRESS = 2, REVIEW = 3, COMPLETED = 4
};
export enum LeaveType {
    Casual = 1, Festive = 2, Sick = 5
};