FROM node:20-alpine AS build
WORKDIR /app

# enable corepack and prepare yarn
RUN corepack enable && corepack prepare yarn@3.6.1 --activate

# copy package manifests and yarn config first for caching
COPY package.json .yarnrc.yml ./
COPY .yarn .yarn
COPY yarn.lock .

RUN yarn install --immutable

# copy the rest of the app and build
COPY . .
ARG VITE_API_BASE=http://localhost:5000
ENV VITE_API_BASE=${VITE_API_BASE}

RUN yarn build

FROM nginx:stable-alpine AS runtime
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
