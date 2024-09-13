FROM node:slim
ENV NODE_ENV=development
WORKDIR /app
COPY package.json .
RUN npm install --verbose
COPY . .
EXPOSE 5173
CMD ["npm", "run", "dev"]