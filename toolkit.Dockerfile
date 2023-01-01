# build base dotnet/aspnet runtime image with kali toolkit
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /kali

ENV DEBIAN_FRONTEND noninteractive
ENV KALI_KEYRING_DEB kali-archive-keyring_2022.1_all.deb

# add sources
RUN apt-get update \
    && apt-get install wget gnupg apt-utils -y \
    && wget https://http.kali.org/kali/pool/main/k/kali-archive-keyring/$KALI_KEYRING_DEB \
    && dpkg -i $KALI_KEYRING_DEB \
    && sh -c "echo 'deb https://http.kali.org/kali kali-rolling main non-free contrib' > /etc/apt/sources.list.d/kali.list" \
    && apt-get update

# install kali toolkit
RUN apt-get install kali-linux-headless -yq -o Dpkg::Options::="--force-confdef" -o Dpkg::Options::="--force-confold"

# cleanup
RUN apt-get clean \
    && rm -rf /tmp/* /var/lib/apt/lists/*