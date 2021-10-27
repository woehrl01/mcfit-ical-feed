FROM gitpod/workspace-base

RUN mkdir $HOME/dotnet_install && cd $HOME/dotnet_install \
    && curl -L https://aka.ms/install-dotnet-preview -o install-dotnet-preview.sh \
    && bash install-dotnet-preview.sh