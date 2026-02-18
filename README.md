
---

# 📡 Device Management & Monitoring System

A real-time **TCP multi-client device monitoring system** built with:

* **Backend:** .NET 10 (C#)
* **Client Simulator:** .NET Console Application
* **Frontend Dashboard:** Angular 21.1.1
* **Communication Protocols:** TCP + WebSockets

---

## 🚀 Overview

This project implements a **multi-client TCP server** that manages device connections, tracks their state, and monitors heartbeats in real time.

An Angular dashboard displays device status updates instantly using WebSockets.

The system fully satisfies all core requirements:

* ✅ TCP Multi-Client Support
* ✅ Device State Management
* ✅ Background Heartbeat Monitoring
* ✅ Concurrency Control (Max 5 Commands)
* ✅ Thread Safety

---

## 🏗 Architecture

### 🔹 Backend (.NET 10)

* TCP Server (Port **5000**)
* WebSocket Server (Port **8080**)
* `ConcurrentDictionary` for device storage
* `SemaphoreSlim(5)` for concurrency control
* Background `HeartbeatMonitor` (non-blocking)
* `Interlocked` for safe active command tracking

### 🔹 Frontend (Angular 21.1.1)

* Standalone components
* WebSocket client connection
* Real-time device dashboard
* Online/Offline badges
* Formatted last heartbeat timestamp
* Auto refresh updates

### 🔹 System Flow

```
TCP Clients → TCP Server → DeviceManager
DeviceManager → WebSocketManager → Angular Dashboard
HeartbeatMonitor → Background Service
```

---

## ⚙ Functional Requirements Implementation

### 1️⃣ TCP Multi-Client Server

* Configurable port (**5000**)
* Supports multiple simultaneous device connections
* Line-based protocol commands:

```
REGISTER|DeviceId
STATUS|DeviceId|RUNNING
STATUS|DeviceId|STOPPED
HEARTBEAT|DeviceId
DISCONNECT|DeviceId
```

---

### 2️⃣ Device State Management

Each device tracks:

* `DeviceId`
* `Current Status`
* `IsOnline`
* `LastHeartbeat`

All device data is stored safely using `ConcurrentDictionary`.

---

### 3️⃣ Heartbeat Monitoring Engine

* Devices send heartbeat every **5 seconds**
* If no heartbeat for **15 seconds** → device automatically marked **OFFLINE**
* Runs in background thread/task
* Does NOT block TCP server

---

### 4️⃣ Concurrency Control

* Maximum **5 commands** execute simultaneously
* Implemented using:

```
SemaphoreSlim(5)
```

* Additional commands wait in queue
* Flood test confirms correct behavior

---

### 5️⃣ Thread Safety

The system prevents race conditions using:

* `ConcurrentDictionary`
* `SemaphoreSlim`
* `Interlocked`

All shared resources are protected.

---

## 🌐 Real-Time Web Dashboard

Angular dashboard features:

* Live device list
* Real-time updates via WebSocket
* Online / Offline indicators
* Last heartbeat display
* Automatic UI refresh

**WebSocket Endpoint:**

```
ws://localhost:8080/ws
```

---

## 🧪 How To Run

### 1️⃣ Start Backend Server

```bash
dotnet run
```

---

### 2️⃣ Start Angular Frontend

```bash
ng serve
```

Open in browser:

```
http://localhost:4200
```

---

### 3️⃣ Start Client Simulator

Run multiple instances:

```bash
dotnet run
```

---

## 🧪 Testing Scenarios

### ✅ Test 1 — Register Device

```
REGISTER|D1
```

Device appears instantly on dashboard.

---

### ✅ Test 2 — Status Update

```
STATUS|D1|RUNNING
```

Dashboard updates in real time.

---

### ✅ Test 3 — Heartbeat Monitoring

Start heartbeat:

* `LastHeartbeat` updates every 5 seconds.

---

### ✅ Test 4 — Auto Offline Detection

Stop heartbeat:

* After 15 seconds → device marked **OFFLINE**

---

### ✅ Test 5 — Flood Test (Concurrency)

Send 20 commands simultaneously.

* Only 5 execute at once.
* Others wait in queue.
* No crashes or race conditions.

---

## 📌 Technologies Used

* .NET 10
* C#
* TCP Sockets
* WebSockets
* Angular CLI 21.1.1
* Node 20.20.0

---

## 🎯 Exam Requirements Coverage

| Requirement                 | Status  |
| --------------------------- | ------  |
| TCP Multi-Client Support    | ✅      |
| Device State Tracking       | ✅      |
| Background Heartbeat Engine | ✅      |
| Max 5 Concurrent Commands   | ✅      |
| Thread Safety               | ✅      |

---


