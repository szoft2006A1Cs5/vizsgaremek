from locust import HttpUser, task
import random

token = None

class ComoveAPILoadTest(HttpUser):
    def on_start(self):
        self.client.verify = False

        with self.client.post(
            "/api/Auth/login",
            json={ "email": "tesztelek@teszt.hu", "password": "NagyTesztElek32" },
            verify=False,
            catch_response=True
        ) as resp:
            if resp.status_code == 200:
                token = resp.json().get("token")
                self.client.headers.update({"Authorization": f"Bearer {token}"})

    @task
    def getUserTest(self):
        self.client.get(f"/api/User/{random.randint(1, 3)}", verify=False)

    @task
    def getVehicles(self):
        self.client.get("/api/Vehicle", verify=False)

    @task
    def getVehicle(self):
        self.client.get(f"/api/Vehicle/{random.randint(1, 3)}", verify=False)

    @task
    def getNotifications(self):
        self.client.get("/api/Notifications", verify=False)

    @task
    def postLogin(self):
        with self.client.post(
            "/api/Auth/login",
            json={"email": "a", "password": "a"}, 
            verify=True, 
            catch_response=True) as resp:
            if resp.status_code == 401:
                resp.success()
