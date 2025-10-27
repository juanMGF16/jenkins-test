# 📖 Escenario Simulado del Sistema de Inventario QR

---

## 👤 Usuarios y Roles

* **Carlos Pérez** → Usuario `admin` → **Administrador General**
  Crea la empresa SENA y gestiona todo el sistema.

* **Laura Gómez** → Usuario `subadmin_centro1` → **Subadministradora**
  Gestiona la **Sucursal Norte**.

* **Andrés Martínez** → Usuario `subadmin_centro2` → **Subadministrador**
  Gestiona la **Sucursal Sur**.

* **Encargados de Zona** (6 personas → 1 por zona):
  Ejemplo: Marta López (`enc_zone_1`) es encargada de la zona *Laboratorio de Redes*.

* **Operativos**:

  * Pedro Díaz (`oper_1`)
  * Juan Torres (`oper_2`)
    Estos dos forman parte de un grupo operativo asignado por un encargado.

* **Verificador**:

  * Ana Ruiz (`verifier`)
    Valida inventarios enviados desde la app.

---

## 🏢 Empresa y Sucursales

* **Empresa:** *SENA – Servicio Nacional de Aprendizaje*
  Industria: **Educación**
  NIT: 900123456-1

* **Sucursal 1:** Centro de Formación Industrial - **Sede Norte**
  Dirección: Cll 100 #10-20, Bogotá
  Subadministradora: **Laura Gómez**

* **Sucursal 2:** Centro de Formación Tecnológica - **Sede Sur**
  Dirección: Av. 68 #50-30, Bogotá
  Subadministrador: **Andrés Martínez**

---

## 🏬 Zonas por Sucursal

### Sucursal Norte

1. **Laboratorio de Redes** – Encargada: Marta López

   * Ítems: Router Cisco, Switch HP
2. **Aula de Informática** – Encargado: Diego Ruiz

   * Ítems: Portátil Dell, Monitor Samsung
3. **Bodega** – Encargada: Sofía Torres

   * Ítems: Escritorio, Silla ergonómica

### Sucursal Sur

4. **Taller de Mecánica** – Encargado: Juan Díaz

   * Ítems: Multímetro, Osciloscopio
5. **Sala de Equipos** – Encargada: Paula Gómez

   * Ítems: Proyector Epson, Televisor LG
6. **Laboratorio de Electrónica** – Encargado: Luis Ramírez

   * Ítems: Cable HDMI, Cable de poder

---

## 📦 Ítems y Categorías

Ejemplos:

* *Router Cisco RV260* → Categoría: **Dispositivos de Comunicación**
* *Escritorio metálico* → Categoría: **Muebles de Oficina**
* *Multímetro UNI-T* → Categoría: **Equipos de Laboratorio**
* *Proyector Epson* → Categoría: **Electrodomésticos**
* *Cable HDMI* → Categoría: **Accesorios y Periféricos**

👉 Cada ítem tiene un **código QR único** (ejemplo: `LABRED-001`) y está en estado inicial **Disponible**.

---

## 👥 Grupos Operativos

* **Grupo 1:** “Inventario Febrero - Laboratorio Redes”

  * Encargado: Marta López
  * Operativos: Pedro Díaz y Juan Torres
  * Periodo: 01/02/2025 – 05/02/2025

* **Grupo 2:** “Inventario Marzo - Taller Mecánica”

  * Encargado: Juan Díaz
  * Operativos: (ninguno adicional aún, pero podría sumarse)
  * Periodo: 10/03/2025 – 12/03/2025

---

## 📋 Inventarios Realizados

1. **Inventario 1 – Laboratorio de Redes (02/02/2025)**

   * Router Cisco → Estado: En orden
   * Switch HP → Estado: Reparación (puerto dañado)
   * Portátil Dell (zona distinta, se escaneó también) → Estado: En orden
     🔎 Observaciones: “Inventario inicial sin novedades excepto un switch.”

2. **Inventario 2 – Taller de Mecánica (11/03/2025)**

   * Multímetro → En orden
   * Osciloscopio → En orden
     🔎 Observaciones: “Inventario de taller - todos los equipos en orden.”

---

## ✅ Verificaciones

* **Inventario 1 (Laboratorio de Redes):**
  Verificado por Ana Ruiz → Resultado: ❌ No conforme.
  Observación: “Switch con puerto dañado; se sugiere reparación y actualización de inventario base.”

* **Inventario 2 (Taller de Mecánica):**
  Verificado por Ana Ruiz → Resultado: ✅ Conforme.
  Observación: “Inventario conforme.”

---

## 🔔 Notificaciones generadas

* Subadministrador Laura Gómez recibe correo de **credenciales iniciales**.
* Operativo Pedro Díaz recibe **recordatorio de inicio de inventario Febrero**.
* Verificador Ana Ruiz recibe notificación in-app: “Inventario 1 en verificación”.

---

## 🎯 En Resumen

* La empresa **SENA** tiene 2 sucursales, cada una con 3 zonas.
* Cada zona tiene un encargado y un inventario base cargado.
* Se hicieron 2 inventarios:

  * Uno con problemas (Switch dañado).
  * Otro sin problemas (todo en orden).
* El flujo completo funciona: **Administrador → Subadmin → Encargados → Operativos → Verificadores**.