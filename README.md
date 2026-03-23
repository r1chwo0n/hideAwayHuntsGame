
# 🎮 Hideaway Hunts: AI Decision-Making Comparison

**Hideaway Hunts** เป็นโปรเจกต์เกมที่พัฒนาด้วย **Unity 6** เพื่อศึกษาและเปรียบเทียบประสิทธิภาพของระบบตัดสินใจของ AI (Bot) ระหว่างการใช้ระบบเงื่อนไขตายตัว (**Rule-based**) และระบบตรรกศาสตร์คลุมเครือ (**Fuzzy Logic**) ในสภาพแวดล้อมการต่อสู้แบบ Third-person

![Unity Version](https://img.shields.io/badge/Unity-6000.0.37f1-black?style=flat&logo=unity)
![Language](https://img.shields.io/badge/Language-C%23-blue?style=flat&logo=csharp)
![Status](https://img.shields.io/badge/Status-Project%20Completed-green)

## 🌟 Key Features
* **Dual-AI System:** สลับโหมดการทำงานของบอทระหว่าง Rule-based และ Fuzzy Logic ได้ทันทีเพื่อเปรียบเทียบพฤติกรรม
* **Fuzzy Logic Engine:** ระบบ AI ที่เลียนแบบความ "ลังเล" และ "การตัดสินใจหน้างาน" ของมนุษย์ผ่าน Membership Functions
* **Perception System:** ระบบการรับรู้สภาพแวดล้อม (Field of View/Detection Range) ที่ส่งผลต่อการตัดสินใจของ AI
* **Third-Person Gameplay:** ประสบการณ์การเล่นที่สมจริงด้วย Assets จาก Mixamo และฉากสภาพแวดล้อมที่ซับซ้อน

## 🧠 AI Comparison Overview
โปรเจกต์นี้มุ่งเน้นการวิเคราะห์ 3 ด้านหลัก:
1.  **System Logic:** ความถูกต้องของการเลือกสถานะ (Form Decision) และการกระทำ (Action State)
2.  **Survival Time:** การวัดประสิทธิภาพเชิงปริมาณของความทนทานในเกม
3.  **Human-like Behavior:** การประเมินความสมจริงและความท้าทายผ่านมุมมองของผู้เล่นจริง

## 📁 Project Structure
```text
Assets/
├── Scripts/
│   ├── Bot/          # Core AI Logics (Fuzzy Engine, Rule-based Brain)
│   ├── Game/         # GameManager, Timers, Spawners
│   ├── Gun/          # Combat Mechanics, Projectiles
│   └── UI/           # Mode Selectors, Game HUD
├── Scenes/           # Main Game & Testing Scenes
└── Polylised/        # Environment Assets (Medieval Desert City)
```

## 🚀 Getting Started

### Prerequisites
* **Unity Hub**
* **Unity Editor 6000.0.37f1 (Unity 6)**
* Visual Studio 2022 (for script editing)

### Installation
1.  Clone the repository:
    ```bash
    git clone https://github.com/r1chwo0n/hideAwayHuntsGame.git
    ```
2.  เปิดโปรเจกต์ผ่าน **Unity Hub**
3.  ไปที่โฟลเดอร์ `Assets/Scenes` และเปิดฉาก **`Home`**
4.  กดปุ่ม **Play** เพื่อเริ่มการทดสอบ

## 🛠️ Built With
* **Game Engine:** [Unity 6](https://unity.com/)
* **Models & Animations:** [Mixamo](https://www.mixamo.com/)
* **Environment:** Polylised - Medieval Desert City
* **Logic:** C# (Custom Fuzzy Logic Engine)

---
**Developed by:** [Your Name/Team Name]  
**Faculty of Engineering, Chiang Mai University (Entaneer)**

