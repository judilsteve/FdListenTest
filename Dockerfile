# BUILD SERVER
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS server-builder

WORKDIR /tmp/FdListenTest
COPY FdListenTest.csproj /tmp/FdListenTest
COPY Program.cs .

RUN dotnet restore
RUN dotnet publish -c Release -o build

# BUILD RUNTIME ENVIRONMENT
FROM mcr.microsoft.com/dotnet/aspnet:6.0

RUN apt-get update
# Last ditch effort to get notify working by runnning
# podman exec -it fd-listen-test /bin/bash -c "echo READY=1 | socat - UNIX-CONNECT:/run/notify/notify.sock"
# after the container starts up
# (not working, socat returns without error but service still stuck in activating phase)
RUN apt-get install -y socat

# Base image sets this to http://+:80. We want to listen only on the systemd file descriptors
ENV ASPNETCORE_URLS=

WORKDIR /FdListenTest
COPY --from=server-builder /tmp/FdListenTest/build /FdListenTest

ENTRYPOINT [ "/FdListenTest/FdListenTest" ]
