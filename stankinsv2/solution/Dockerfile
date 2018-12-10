
#FROM microsoft/dotnet:aspnetcore-runtime
ARG Build.ArtifactStagingDirectory
RUN echo "staging dir $Build.ArtifactStagingDirectory"
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /root/  
COPY $(Build.ArtifactStagingDirectory)\\win10-x64 .
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000/tcp
CMD ["StankinsAliveMonitor.exe"]  
