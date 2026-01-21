from locust import HttpUser, task
import random

class ComoveAPILoadTest(HttpUser):
    def on_start(self):
        self.client.verify = False

    @task
    def getUserTest(self):
        self.client.get(f"/api/User/{random.randint(1, 3)}", verify=False)

    @task
    def getVehicles(self):
        self.client.get("/api/Vehicle", verify=False)

    @task
    def getVehicle(self):
        self.client.get(f"/api/Vehicle/{random.randint(1, 3)}", verify=False)