# dotnet_version seems to be set by something else with the extra patch version
# number like 2.2.0, where as DOTNET_VER is just 2.2, which we use with rage's folder
# specification
# Usually you just set this as a build arg
# If you change this, also check to make sure you change the CMD instruction to the right folder
ARG DOTNET_VER=3.1

FROM mcr.microsoft.com/dotnet/core/sdk:${DOTNET_VER} AS build-env
ARG DOTNET_VER
WORKDIR /app

# Source
COPY . .

# Build
# No warnings on dll version mismatches. Rage Clientside DLL expects a different
# version of JSON than the server version but that's fine.
RUN dotnet publish -c Release OwlV.sln /p:TreatWarningsAsErrors=true /warnaserror /nowarn:msb3277 /maxcpucount:1 /p:BuildInParallel=false \
	# Cooker
	&& dotnet Source/Cooker/bin/Release/netcoreapp${DOTNET_VER}/publish/Cooker.dll

# Use runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:${DOTNET_VER}
ARG DOTNET_VER

# Git hash
ARG CI_COMMIT_SHA
ENV GIT_COMMIT_SHA $CI_COMMIT_SHA
# Latest version
ARG CI_LATEST_TAG
ENV GIT_LATEST_TAG $CI_LATEST_TAG

# Install deps for server
RUN apt-get update && apt-get install -y \
	libc6-dev \
	libunwind8 \
	libatomic1 \
	&& rm -rf /var/lib/apt/lists/*

# Creates a system user with no password and the /app home dir
RUN useradd -m -d /app v
USER v

WORKDIR /app

# Install server - Up to date with prerelease again
RUN curl -O https://cdn.rage.mp/updater/prerelease/server-files/linux_x64.tar.gz \
	&& tar --strip-components=1 -xvf linux_x64.tar.gz \
	&& rm -f linux_x64.tar.gz \
	&& chmod +x ragemp-server \
	# Copy the included runtime dll's because rage's are broken
	# dotnet_version seems to be set by something else with the extra patch version
	# number like 2.2.0, where as DOTNET_VER is just 2.2, which we use
	# && cp /usr/share/dotnet/shared/Microsoft.NETCore.App/$DOTNET_VERSION*/* ./bridge/runtime/ \
	# Create the logs directory as the user v
	&& mkdir /app/logs

COPY --chown=v --from=build-env /app/Output/Release ./
COPY --chown=v --from=build-env /app/RageDeps/Bootstrapper.dll ./dotnet/runtime/

RUN chmod +x ./netcoreapp3.1

ENV DOTNET_VER ${DOTNET_VER}
# We can't use dotnet ver here because we don't want to create a shell, (otherwise it fucks with signal handling)
CMD ["dotnet", "netcoreapp3.1/publish/owl_boot.dll"]
