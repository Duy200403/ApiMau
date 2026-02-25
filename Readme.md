các bước chạy migration:
  b1: dotnet ef migrations add MigrationName 
  b2: dotnet ef database update
Upload file trong cùng project: tạo folder website_data/images và website_data/document

publish lên sever:
   b1: Chạy câu lệnh sau : dotnet publish -c Release -o ./publish
   b2: coppy các file trong thư mục publish được tạo ra trừ các file appsetting.json và web.config.