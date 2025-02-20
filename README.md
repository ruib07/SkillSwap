To run this project, you have to complete the following steps:

- Create a .env file on the frontend and put the VITE_API_BASE_URL to your backend url
- Create the database following these steps:
  - Delete Migrations folder in SkillSwap.EntityConfiguration
  - On the terminal, "Add-Migration migration-name"
  - On the terminal, "Update-Database"
- You need to be with SkillSwap.Server as the StartUp Project and the terminal in the SkillSwap.EntityConfiguratio project
- Add a appsettings.json on your SkillSwap.Server with the following structure:
```json
{
  "ConnectionStrings": {
    "SkillSwapDb": "your connection string"
  },
  "Jwt": {
    "Issuer": "your issuer",
    "Audience": "your audience",
    "Key": "your key"
  },
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "your email",
    "Password": "your password",
    "EnableSsl": true
  },
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Forwarded-For",
    "ClientIdHeader": "X-ClientId",
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1s",
        "Limit": 5
      }
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```
- Then if you´re using VS2022, you can create a profile that start the SkillSwap.Server and the skillswap.client at the same time in the "Configure Startup Projects"

With this steps, you can have the project on your computer without any problem. Have a great day 😄

Home Page (Learners):
![HomePageLearner](https://github.com/user-attachments/assets/1178b5b2-2ae3-4c57-ac42-c9f7b0e33901)

Home Page (Mentors):
![HomePageMentor](https://github.com/user-attachments/assets/39787ca4-5ce0-4736-adaa-c2ee56171d14)

Signup Page:
![Signup](https://github.com/user-attachments/assets/93aa0062-6956-4bdc-b5e1-5b17b1385307)

Signin Page:
![Signin](https://github.com/user-attachments/assets/6803d2de-7f0f-436e-9c71-cb3636486b43)

Recover Password/Send Email Page:
![SendEmail](https://github.com/user-attachments/assets/10d7baaf-fd3d-4255-a8b2-a67d869dac30)

Recover Password/Change Password Page:
![ChangePassword](https://github.com/user-attachments/assets/f39d9031-8e62-4919-8d11-75593a3048c7)

Skills Page (Mentors):
![Skills](https://github.com/user-attachments/assets/db6e8af9-1772-412b-892b-1eae1e092537)

Skill Details Page (Mentors):
![SkillDetails](https://github.com/user-attachments/assets/6b66a0bb-ae90-474e-87f2-9a45c247088d)

Create Skill Page (Mentors):
![CreateSkill](https://github.com/user-attachments/assets/2678e54c-e405-4826-9277-48003002da3c)

Mentors page:
![Mentors](https://github.com/user-attachments/assets/b4fc34b0-cdaa-4d1c-84c4-54248693958f)

Mentorship Requests Page (Learners):
![MentorshipRequestsLearner](https://github.com/user-attachments/assets/7d8dfe92-8eda-4327-bf1c-f977ff76b71c)

Mentorship Requests Page (Mentors):
![MentorshipRequestsMentor](https://github.com/user-attachments/assets/b457210b-56fd-4977-8ec4-5405c1d09449)

Create Mentorship Request Page (Mentors):
![CreateMentorshipRequest](https://github.com/user-attachments/assets/4dcba822-7a89-49ff-91b5-1a4b73519a9a)

Sessions Page:
![Sessions](https://github.com/user-attachments/assets/4f1ac9be-70ca-4046-929f-3ebacd170a42)

Session Details Page:
![SessionDetails](https://github.com/user-attachments/assets/f4f4657c-6d77-41aa-841e-93c830d79f08)

Create Session Page (Mentors):
![CreateSession](https://github.com/user-attachments/assets/d34f94c9-f65f-44dd-b51e-ea0a6583a1e4)

Profile/User Information Page:
![UserInfo](https://github.com/user-attachments/assets/d3adcbe5-caf6-4040-99f7-d32af0232bcc)

Profile/Learner Reviews Page:
![LearnerReviews](https://github.com/user-attachments/assets/b16ddc19-90ac-4a10-b798-6070c9aadec0)

Profile/Learner Payments Page:
![LearnerPayments](https://github.com/user-attachments/assets/4125c8fb-a235-40ae-b58c-7f128c52c86f)

Profile/Mentor Skills Page:
![MentorSkills](https://github.com/user-attachments/assets/79e2d2c7-4002-4db3-8b7b-0589335fa9fa)

Profile/Mentor Payments Page:
![MentorPayments](https://github.com/user-attachments/assets/c79cff22-aca3-4568-b6b8-6403667fe175)









