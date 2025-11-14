🧩 ModbusTcpServer – Servidor Modbus/TCP de prueba (.NET 8 + NModbus)
📘 Descripción general

ModbusTcpServer es una aplicación de consola desarrollada en .NET 8 que emula el comportamiento de un dispositivo Modbus/TCP.

Su propósito es ofrecer un entorno de simulación local o en laboratorio, para validar el funcionamiento de aplicaciones Modbus cliente (por ejemplo, software de control, servicios de adquisición de datos, HMIs o middleware) sin necesidad del hardware físico.

⚙️ Características principales

✅ Implementa un servidor Modbus/TCP compatible con el estándar (puerto 502).

✅ Utiliza la librería NModbus 3.0.81.

✅ Simula registros de tipo Holding Registers (4x) e Input Registers (3x).

✅ Permite configurar la IP, puerto y Unit ID mediante argumentos de línea de comandos.

✅ Incluye un mecanismo de validación de IP:
si la dirección solicitada no está asignada en el sistema, se usa automáticamente 0.0.0.0 (todas las interfaces).

✅ Simula variación dinámica de datos (ej. RPM).

✅ Compatible con clientes Modbus de escritorio o industriales (EasyModbus, PLCs, SCADA, etc.).

🧠 Objetivo técnico

Simular un dispositivo remoto (por ejemplo, un PLC) para probar localmente:

Conexión Modbus/TCP de software de escritorio o servicios Windows.

Lógica de lectura/escritura de tags.

Validación de drivers, escalas y tipos de datos.

Integración entre el software de control y el middleware de comunicación sin depender de hardware físico.

🖥️ Requisitos
Componente	Requisito
.NET SDK	.NET 8
Paquete NuGet	NModbus 3.0.81
Permisos	Ejecución como Administrador si se usa el puerto 502
IP asignada	IP fija o loopback virtual (192.168.127.254 recomendado)
Firewall	Permitir entrada TCP en el puerto 502
🚀 Ejecución

Desde la carpeta del proyecto:

dotnet run -- --ip=192.168.127.254 --port=502 --unit=1

Argumentos opcionales
Parámetro	Descripción	Valor por defecto
--ip=	IP local donde escuchar	192.168.127.254
--port=	Puerto TCP de escucha	502
--unit=	Unit ID Modbus	1

Si la IP no está asignada al sistema, el servidor mostrará una advertencia y escuchará en 0.0.0.0 (todas las interfaces).

🔁 Ejemplo de uso

Ejecutar el servidor:

[WARN] La IP 192.168.127.254 no está asignada a esta máquina.
[INFO] Escuchando en 0.0.0.0 (todas las interfaces locales)...
[Modbus TCP Server] Binding 0.0.0.0:502 (UnitId=1)
Servidor escuchando... (Ctrl+C para salir)


Desde un cliente Modbus (por ejemplo, EasyModbus):

var client = new ModbusClient("192.168.127.254", 502);
client.Connect();
var values = client.ReadHoldingRegisters(0, 3);
// → [1500, 250, 1234]


El servidor muestra en consola los eventos de escritura o actualización:

[12:45:07] WRITE HoldingRegister @ 1 => [900]

🧰 Microsoft KM-TEST Loopback Adapter
🧩 ¿Qué es?

Microsoft KM-TEST Loopback Adapter (o Adaptador de bucle invertido de Microsoft) es un driver de red virtual incluido en Windows que permite crear una interfaz de red simulada.
Funciona como una tarjeta de red ficticia, sin hardware físico.

🎯 ¿Para qué se usa en este proyecto?

El adaptador loopback permite asignar una IP fija (por ejemplo 192.168.127.254) a una interfaz virtual, de modo que:

El servidor Modbus pueda escuchar en esa IP como si fuera un equipo remoto.

Los clientes Modbus (en la misma PC o VM) puedan conectarse a esa IP sin conflictos de red.

Se simule fielmente la arquitectura de comunicación de los equipos reales.

En resumen:

El loopback adapter actúa como el “puerto Ethernet virtual” del MOXA simulado.

🧾 Cómo instalarlo
🔹 Método 1 – Desde Administrador de dispositivos

Abrir Administrador de dispositivos → Menú Acción → Agregar hardware heredado.

Elegir Instalar el hardware seleccionado manualmente → Adaptadores de red.

Fabricante: Microsoft
Controlador: Microsoft KM-TEST Loopback Adapter

Una vez instalado, renombrarlo (por ejemplo, Loopback MOXA).

🔹 Método 2 – Desde PowerShell (Windows 10+)

Ejecutar como administrador:

pnputil /add-driver "C:\Windows\INF\netloop.inf" /install

🔹 Asignar IP estática

Panel de control → Centro de redes → Cambiar configuración del adaptador.

Propiedades → Protocolo de Internet versión 4 (TCP/IPv4) → Usar la siguiente dirección IP.

Dirección IP: 192.168.127.254
Máscara: 255.255.255.0
Puerta de enlace: (vacío)


Guardar.

Ahora podés ejecutar el servidor usando esa IP sin error.

📦 Estructura del proyecto
SimuMoxa/
 ┣ Program.cs                 # Lógica principal del servidor
 ┣ ModbusTcpServer.csproj     # Configuración del proyecto .NET 8
 ┣ README.md                  # Este archivo
 ┣ /bin, /obj                 # Salida de compilación

🧩 Ejemplo de registros simulados
Dirección (base 1)	Tipo	Descripción	Valor inicial
40001	Holding Register	RPM actual	1500
40002	Holding Register	Presión de gas (×0.1 bar)	250
40003	Holding Register	Caudal (×0.1 L/s)	1234
30011	Input Register	Segundo actual del sistema	variable
🧱 Roadmap sugerido

 Agregar configuración persistente vía appsettings.json.

 Logging de escritura en archivo (logs/modbus.log).

 Múltiples Unit ID (simular varios equipos en una misma instancia).

 Convertir a Windows Service (ejecución automática en arranque).
