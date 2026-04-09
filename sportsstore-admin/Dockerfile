# React Admin Dashboard

FROM node:20-alpine AS builder
WORKDIR /app

COPY package*.json ./
<<<<<<< HEAD
RUN npm install
=======
RUN npm ci
>>>>>>> d53dbaa649fb2b7845b5dc91ce1f794377a85d00

COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=builder /app/build /usr/share/nginx/html
<<<<<<< HEAD
EXPOSE 80
=======
EXPOSE 3000
>>>>>>> d53dbaa649fb2b7845b5dc91ce1f794377a85d00
CMD ["nginx", "-g", "daemon off;"]
