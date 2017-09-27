FROM microsoft/aspnetcore:2.0.0
ARG source=.
WORKDIR /app
COPY $source .
EXPOSE 80
ENTRYPOINT ["dotnet", "SmarterBalanced.SampleItems.Web.dll"]

