
#FROM microsoft/dotnet:aspnetcore-runtime
FROM microsoft/dotnet:2.1-aspnetcore-runtime
ARG BuildArtifactStagingDirectory
RUN echo "staging dir ${BuildArtifactStagingDirectory}"
WORKDIR /root/  
COPY $(BuildArtifactStagingDirectory)\\win10-x64 .
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000/tcp
CMD ["StankinsAliveMonitor.exe"]  
