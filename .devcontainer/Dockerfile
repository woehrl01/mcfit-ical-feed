# based on https://github.com/microsoft/vscode-dev-containers/blob/v0.194.3/containers/dotnet/.devcontainer/base.Dockerfile
# [Choice] .NET version: 6.0, 5.0, 3.1, 2.1
ARG VARIANT="6.0"
FROM mcr.microsoft.com/dotnet/sdk:${VARIANT}-focal

# Copy library scripts to execute
COPY library-scripts/*.sh library-scripts/*.env /tmp/library-scripts/

# [Option] Install zsh
ARG INSTALL_ZSH="true"
# [Option] Upgrade OS packages to their latest versions
ARG UPGRADE_PACKAGES="true"
# Install needed packages and setup non-root user. Use a separate RUN statement to add your own dependencies.
ARG USERNAME=vscode
ARG USER_UID=1000
ARG USER_GID=$USER_UID
RUN bash /tmp/library-scripts/common-debian.sh "${INSTALL_ZSH}" "${USERNAME}" "${USER_UID}" "${USER_GID}" "${UPGRADE_PACKAGES}" "true" "true" \
    && apt-get clean -y && rm -rf /var/lib/apt/lists/*

# Remove library scripts for final image
RUN rm -rf /tmp/library-scripts