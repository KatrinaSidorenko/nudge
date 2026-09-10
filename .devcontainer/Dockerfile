# Uses Ubuntu 24.04 LTS (Noble Numbat)
FROM mcr.microsoft.com/devcontainers/base:ubuntu-24.04

# 1. Install Node.js (for Claude Code & GitHub MCP)
RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get install -y nodejs git curl wget \
    && apt-get clean -y && rm -rf /var/lib/apt/lists/*

# 2. Install .NET 10 SDK via Microsoft script
RUN wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh \
    && chmod +x ./dotnet-install.sh \
    && ./dotnet-install.sh --channel 10.0 --install-dir /usr/share/dotnet \
    && ln -s /usr/share/dotnet/dotnet /usr/usr/bin/dotnet || true

# Add .NET to PATH
ENV DOTNET_ROOT=/usr/share/dotnet
ENV PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools

# 3. Install Claude Code
# RUN npm install -g @anthropic-ai/claude-code -- old one
# The installer only accepts [stable|latest|VERSION] as its one positional arg
# (no confirmation flag exists/is needed for a non-interactive `curl | bash`).
RUN curl -fsSL https://claude.ai/install.sh | bash -s -- stable

# Ensure Claude Code binary directory is in PATH
ENV PATH="/root/.local/bin:/home/vscode/.local/bin:${PATH}"


# Pre-install GitHub CLI (gh)
RUN mkdir -p -m 755 /etc/apt/keyrings \
    && wget -qO- https://cli.github.com/packages/githubcli-archive-keyring.gpg | tee /etc/apt/keyrings/githubcli-archive-keyring.gpg > /dev/null \
    && chmod go+r /etc/apt/keyrings/githubcli-archive-keyring.gpg \
    && echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages stable main" | tee /etc/apt/sources.list.d/github-cli.list > /dev/null \
    && apt-get update \
    && apt-get install -y gh \
    && apt-get clean -y && rm -rf /var/lib/apt/lists/*
