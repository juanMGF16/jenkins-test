# 🔐 MÓDULO DE SEGURIDAD - SISTEMA DE INVENTARIOS

## 👥 ROLES DEL SISTEMA

### 🔴 **ADMINISTRADOR**
**Descripción:** Administrador general del sistema. Gestiona empresas y sucursales.

**Acceso a Módulos:**
- ✅ **Administración**
- ✅ **Seguridad**

**Formularios y Permisos:**

**📋 Gestión de Empresas**
- Crear, Leer, Actualizar, Eliminar, Exportar
- Consultar Consolidado, Configurar Sistema

**🏢 Gestión de Sucursales** 
- Crear, Leer, Actualizar, Eliminar, Exportar
- Asignar Sucursales, Generar Credenciales, Consultar Consolidado

**👤 Gestión de Usuarios**
- Crear, Leer, Actualizar, Eliminar
- Asignar Roles, Generar Credenciales, Resetear Password

**🛡️ Gestión de Roles**
- Crear, Leer, Actualizar, Eliminar, Asignar Roles

**👨‍💼 Perfil de Usuario**
- Leer, Actualizar, Actualizar Perfil, Cambiar Password

**🔔 Notificaciones**
- Leer, Gestionar Notificaciones

---

### 🟠 **SUBADMINISTRADOR** 
**Descripción:** Administrador de sucursal. Gestiona zonas y usuarios de la sucursal asignada.

**Acceso a Módulos:**
- ✅ **Administración** 
- ✅ **Operaciones**
- ✅ **Reportes**
- ✅ **Seguridad**

**Formularios y Permisos:**

**🗺️ Gestión de Zonas**
- Crear, Leer, Actualizar, Eliminar, Exportar
- Asignar Zonas, Generar Credenciales

**👷 Gestión de Operativos**
- Crear, Leer, Actualizar, Eliminar, Exportar, Gestionar Operativos

**📊 Reportes de Inventario**
- Generar Reportes, Consultar Historial, Ver Estadísticas

**🔍 Consultas Generales**
- Leer, Generar Reportes, Consultar Historial

**📈 Historial de Inventarios**
- Leer, Consultar Historial, Ver Estadísticas

**👤 Gestión de Usuarios** *(solo de su sucursal)*
- Crear, Leer, Actualizar, Generar Credenciales, Resetear Password

**👨‍💼 Perfil de Usuario**
- Leer, Actualizar, Actualizar Perfil, Cambiar Password

**🔔 Notificaciones**
- Leer, Enviar Notificaciones, Enviar Recordatorios, Gestionar Notificaciones

---

### 🟡 **ENCARGADO_ZONA**
**Descripción:** Encargado de zona específica. Gestiona inventario base y grupos operativos.

**Acceso a Módulos:**
- ✅ **Inventario**
- ✅ **Operaciones** 
- ✅ **Verificación**
- ✅ **Reportes**
- ✅ **Seguridad**

**Formularios y Permisos:**

**📦 Gestión de Items**
- Crear, Leer, Actualizar, Eliminar, Exportar, Modificar Estados Item

**📥 Carga de Inventario Base**
- Importar, Procesar Solicitudes

**🔲 Generación de QR**
- Generar QR, Imprimir QR

**🏷️ Gestión de Categorías**
- Crear, Leer, Actualizar, Eliminar, Gestionar Categorías

**📊 Gestión de Estados**
- Crear, Leer, Actualizar, Eliminar, Gestionar Estados

**👷 Gestión de Operativos**
- Crear, Leer, Actualizar, Eliminar, Exportar, Gestionar Operativos

**👥 Gestión de Grupos Operativos**
- Crear, Leer, Actualizar, Eliminar, Crear Grupos Operativos, Asignar Períodos

**📝 Solicitudes de Actualización**
- Leer, Procesar Solicitudes

**📊 Reportes de Inventario**
- Generar Reportes, Consultar Historial, Ver Estadísticas

**🔍 Consultas Generales**
- Leer, Generar Reportes, Consultar Historial

**📈 Historial de Inventarios**
- Leer, Consultar Historial, Ver Estadísticas

**👨‍💼 Perfil de Usuario**
- Leer, Actualizar, Actualizar Perfil, Cambiar Password

**🔔 Notificaciones**
- Leer, Enviar Notificaciones, Enviar Recordatorios, Gestionar Notificaciones

---

### 🟢 **OPERATIVO**
**Descripción:** Aprendiz que realiza inventarios mediante escaneo QR móvil.

**Acceso a Módulos:**
- ✅ **Operaciones**
- ✅ **Seguridad** *(limitado)*

**Formularios y Permisos:**

**📱 Ejecución de Inventario**
- Ejecutar Inventario, Escanear QR, Modificar Estados Item
- Finalizar Inventario, Generar Invitación, Unirse Inventario

**🔲 Escaneo QR**
- Escanear QR, Modificar Estados Item

**👨‍💼 Perfil de Usuario**
- Leer, Actualizar Perfil, Cambiar Password

**🔔 Notificaciones**
- Leer, Gestionar Notificaciones

---

### 🔵 **VERIFICADOR**
**Descripción:** Valida y confirma inventarios realizados por operativos.

**Acceso a Módulos:**
- ✅ **Verificación**
- ✅ **Reportes**
- ✅ **Seguridad** *(limitado)*

**Formularios y Permisos:**

**✅ Verificación de Inventarios**
- Verificar Inventario, Comparar Inventarios, Aprobar Inventario
- Rechazar Inventario, Solicitud Actualización

**📝 Solicitudes de Actualización**
- Leer, Solicitud Actualización

**📊 Reportes de Inventario**
- Generar Reportes, Consultar Historial, Ver Estadísticas

**🔍 Consultas Generales**
- Leer, Generar Reportes, Consultar Historial

**📈 Historial de Inventarios**
- Leer, Consultar Historial, Ver Estadísticas

**👨‍💼 Perfil de Usuario**
- Leer, Actualizar Perfil, Cambiar Password

**🔔 Notificaciones**
- Leer, Enviar Notificaciones, Gestionar Notificaciones

---

## 🏗️ ESTRUCTURA DE MÓDULOS

### 📋 **ADMINISTRACIÓN**
- Gestión de Empresas
- Gestión de Sucursales  
- Gestión de Zonas

### 📦 **INVENTARIO**
- Gestión de Items
- Carga de Inventario Base
- Generación de QR
- Gestión de Categorías
- Gestión de Estados

### 📱 **OPERACIONES**
- Gestión de Operativos
- Gestión de Grupos Operativos
- Ejecución de Inventario
- Escaneo QR

### ✅ **VERIFICACIÓN**
- Verificación de Inventarios
- Solicitudes de Actualización

### 📊 **REPORTES**
- Reportes de Inventario
- Consultas Generales
- Historial de Inventarios

### 🛡️ **SEGURIDAD**
- Gestión de Usuarios
- Gestión de Roles
- Perfil de Usuario
- Notificaciones

---

## 🔗 FLUJO JERÁRQUICO

```
ADMINISTRADOR
    ↓ gestiona empresas y sucursales
    ↓ crea subadministradores
    
SUBADMINISTRADOR  
    ↓ gestiona zonas de su sucursal
    ↓ crea encargados de zona
    
ENCARGADO_ZONA
    ↓ gestiona inventario base
    ↓ crea grupos operativos
    ↓ asigna operativos y verificadores
    
OPERATIVO ← → VERIFICADOR
    ↓ ejecuta inventarios    ↓ valida inventarios
    ↓ escanea QR            ↓ aprueba/rechaza
```

Este sistema permite un control granular de permisos donde cada rol tiene acceso específico a las funcionalidades que necesita para cumplir sus responsabilidades en el proceso de inventarios.