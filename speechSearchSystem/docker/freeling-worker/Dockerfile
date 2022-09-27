
FROM maven:3.8.6-openjdk-18-slim as build

COPY worker/src /home/app/src
COPY worker/pom.xml /home/app

RUN mvn -f /home/app/pom.xml clean package

FROM openjdk:18-alpine
COPY --from=build /home/app/target/MavenFreelingWorker.jar /usr/local/lib/MavenFreelingWorker.jar
ENTRYPOINT ["java", "-jar", "/usr/local/lib/MavenFreelingWorker.jar"]