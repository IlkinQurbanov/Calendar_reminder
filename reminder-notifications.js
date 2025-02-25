// reminder-notifications.js
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

connection.on("ReceiveNotification", (reminder) => {
    if (Notification.permission === "granted") {
        const notification = new Notification(reminder.title, {
            body: reminder.description,
            icon: "/reminder-icon.png"
        });

        notification.onclick = function () {
            window.focus();
            this.close();
        };
    }
});

async function startConnection() {
    try {
        await connection.start();
        console.log("SignalR connected.");

        // Request notification permission
        if (Notification.permission !== "granted") {
            Notification.requestPermission();
        }
    } catch (err) {
        console.error("Error connecting to SignalR hub:", err);
        setTimeout(startConnection, 5000);
    }
}

startConnection();