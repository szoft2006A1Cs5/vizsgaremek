from enum import verify
from locust import HttpUser, task
import random

class ComoveAPILoadTest(HttpUser):
    def on_start(self):
        self.client.verify = False

        self.client.post(
            "/api/Auth/login",
            json={ "email": "tesztelek@teszt.hu", "password": "NagyTesztElek32" },
            verify=False
        )

    @task
    def getUserTest(self):
        self.client.get(f"/api/User/{random.randint(1, 3)}", verify=False)

    @task
    def getVehicles(self):
        self.client.get("/api/Vehicle", verify=False)

    @task
    def getOwnedVehicles(self):
        self.client.get(f"/api/Vehicle/Owned", verify=False)

    @task
    def getVehicle(self):
        self.client.get(f"/api/Vehicle/{random.randint(1, 3)}", verify=False)

    @task
    def getAvailabilities(self):
        self.client.get(f"/api/Vehicle/1/Availability", verify=False)

    @task
    def getImages(self):
        self.client.get(f"/api/Vehicle/{random.randint(1, 3)}/Image", verify=False)

    @task
    def getRentals(self):
        self.client.get(f"/api/Rental", verify=False)

    @task 
    def getOwnedRentals(self):
        self.client.get(f"/api/Rental/Owned", verify=False)

    @task
    def getNotifications(self):
        self.client.get(f"/api/User/1/Notification", verify=False)

    @task
    def postLogin(self):
        with self.client.post(
            "/api/Auth/login",
            json={"email": "a", "password": "a"}, 
            verify=True, 
            catch_response=True) as resp:
            if resp.status_code == 401:
                resp.success()
