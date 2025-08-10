# Chrome WPF

**Chrome WPF** is a desktop application built using **WPF (.NET)**, providing a modern, customizable user interface with advanced window styling and UX enhancements.  
It can serve as a template for creating elegant, production-grade Windows applications.

---

## 📂 Project Structure

```
Chrome_WPF/
├── Chrome_WPF.sln          # Solution file
├── Chrome_WPF/             # Main WPF application project
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── Constants
│   ├── Helpers
│   ├── Models
│   ├── Resources
│   ├── Services
│   ├── ViewModels
│   ├── Views
│   ├── ViewSelectors
└── README.md
```

---

## ✨ Features

- 🎨 **Custom window chrome** (custom title bar, buttons for minimize/maximize/close)  
- 🌓 **Custom styles/themes** (light/dark mode, accent colors)  
- 🖱 **Drag-to-move & custom resizing** without system chrome  
- 🏗 **MVVM-ready** structure separating UI and logic

---

## 🛠 Technologies & Tools

- **.NET (WPF)** – UI framework  
- **C#** – application logic  
- **XAML** – UI layout & styling  

---

## 🚀 Getting Started

### 1️⃣ Clone the Repository
```bash
git clone https://github.com/lntb1712/Chrome_WPF.git
cd Chrome_WPF
```

### 2️⃣ Open in Visual Studio

### 3️⃣ Build & Run
- Press **F5** or **Run** in Visual Studio

---

## 🎯 Customizing Window Chrome

Example: Remove default title bar & create custom buttons

```xml
<Window
    x:Class="Chrome_WPF.MainWindow"
    WindowStyle="None"
    AllowsTransparency="True"
    Background="Transparent">
    <!-- Custom chrome layout -->
</Window>
```

---

## 🤝 Contributing

1. Fork the repo  
2. Create a feature branch  
3. Commit your changes  
4. Open a Pull Request  

---

## 📧 Contact

**Author:** Thanh Bình Lê Nguyễn  
**GitHub:** [@lntb1712](https://github.com/lntb1712)  
