<a name="inicio"></a>
NopCommerce- módulo Todo Pago (v1.0.0)
============

Plug in para la integración con gateway de pago <strong>Todo Pago</strong>
- [Consideraciones Generales](#consideracionesgenerales)
- [Instalación](#instalacion)
- [Configuración](#configuracion)
 - [Activación](#activacion)
 - [Configuración plug in](#confplugin)
 - [Obtener datos de configuracion](#getcredentials)
 - [Configuración de Maximo de Cuotas](#maxcuotas)
- [Características](#features) 
 - [Consulta de transacciones](#constrans)
 - [Devoluciones](#devoluciones)
- [Tablas de referencia](#tablas)
- [Versiones disponibles](#availableversions)

<a name="consideracionesgenerales"></a>
## Consideraciones Generales
El plug in de pagos de <strong>Todo Pago</strong>, provee a las tiendas NopCommerce de un nuevo m&eacute;todo de pago, integrando la tienda al gateway de pago.
La versión de este plug in esta testeada en .NET 4.5 con NopCommerce 3.7.0

<a name="instalacion"></a>
## Instalación
1. Descomprimir el archivo nopcommerce-plugin-master.zip. 
2. Copiar carpeta Payments.TodoPago al directorio de plugins de NopCommerce ("raíz de NopCommerce"Presentation\Nop.Web\Plugins). 
3. Dentro del dashboard admin de la tienda en Configuration -> Plugins -> Local Plugins, hacer click sobre Reload list of plugins.</br>
![imagen de configuracion](https://raw.githubusercontent.com/TodoPago/imagenes/master/nopcommerce/reload.PNG)</br>
4. Buscar el plugin <strong>TodoPago</strong> en la lista y hacer click sobre Install.</br>
![imagen de configuracion](https://raw.githubusercontent.com/TodoPago/imagenes/master/nopcommerce/install.PNG)</br>

[<sub>Volver a inicio</sub>](#inicio)

<a name="configuracion"></a>
##Configuración

<a name="activacion"></a>
####Activación
La activación se realiza como cualquier plugin de NopCommerce: Desde Configuration -> Payment -> Payment Methods.<br />
Buscar el plugin <strong>TodoPago</strong> en la lista y hacer click sobre Edit. 
Una vez hecho esto marcar el checkbox de activacion y hacer click sobre Update. </br>
![imagen de configuracion](https://raw.githubusercontent.com/TodoPago/imagenes/master/nopcommerce/activar.PNG)</br>

<a name="confplugin"></a>  
####Configuración plug in
Para llegar al menu de configuración del plugin ir a: Configuration -> Payment -> Payment Methods.
Buscar el plugin <strong>TodoPago</strong> en la lista y hacer click sobre Configure.</br>
![imagen de configuracion](https://raw.githubusercontent.com/TodoPago/imagenes/master/nopcommerce/configure.PNG)</br>
<sub></br><em>Menú ambiente</em></br></sub>
![imagen de configuracion](https://raw.githubusercontent.com/TodoPago/imagenes/master/nopcommerce/developer.PNG)</br>
![imagen de configuracion](https://raw.githubusercontent.com/TodoPago/imagenes/master/nopcommerce/production.PNG)</br>
<sub></br><em>Meenú estados</em></br></sub>
![imagen de configuracion](https://raw.githubusercontent.com/TodoPago/imagenes/master/nopcommerce/orderStatus.PNG)</br>

- Estado de transacción iniciada: Se setea luego de completar los datos de facturación y presionar el botón "Realizar el pedido".
- Estado de transacción aprobada: Se setea luego de volver del formulario de pago de Todo Pago y se obtiene una confirmación del pago.
- Estado de transacción rechazada: Se setea luego de volver del formulario de pago de Todo Pago y se obtiene un rechazo del pago.
</br>

[<sub>Volver a inicio</sub>](#inicio)

<a name="getcredentials"></a>
####Obtener datos de configuracion
Se puede obtener los datos de configuracion del plugin con solo loguearte con tus credenciales de Todopago.</br>
a. Ir a la opcion Credenciales</br>
b. Loguearse con el mail y password de Todopago.</br>
c. Los datos se cargaran automaticamente en los campos Merchant ID y Security code en el ambiente correspondiente y solo hay que hacer click en el boton guardar datos y listo.</br>
![imagen de configuracion](https://raw.githubusercontent.com/TodoPago/imagenes/master/nopcommerce/credential.PNG)</br>
[<sub>Volver a inicio</sub>](#inicio)

<a name="maxcuotas"></a>
####Configuración de Maximo de Cuotas
Se puede configurar la cantidad máxima de cuotas que ofrecerá el formulario de TodoPago con el campo Numero máximo de cuotas. Para que se tenga en cuenta este valor se debe habilitar el checkbox Habilitar/Desabilitar cantidad de cuotas y tomará el valor fijado para máximo de cuotas. En caso que esté habilitado el campo y no haya un valor puesto para las cuotas se tomará el valor 12 por defecto.
<br />
[<sub>Volver a inicio</sub>](#inicio)

<a name="features"></a>
## Características
 - [Consulta de transacciones](#constrans)
 - [Devoluciones](#devoluciones)
 
<br />
<a name="constrans"></a>
#### Consulta de Transacciones
Se puede consultar <strong>on line</strong> las características de la transacci&oacute;n en el sistema de Todo Pago al hacer click en el menu sobre **TodoPago**.<br />
![imagen de configuracion](https://raw.githubusercontent.com/TodoPago/imagenes/master/nopcommerce/status.PNG)</br>
Hacer click en "View" sobre la orden deseada.<br />
![imagen de configuracion](https://raw.githubusercontent.com/TodoPago/imagenes/master/nopcommerce/statusDetail.PNG)</br>
<br />
[<sub>Volver a inicio</sub>](#inicio)
</br>

<a name="devoluciones"></a>
#### Devoluciones
Es posible realizar devoluciones o reembolsos mediante el procedimiento habitual de NopCommerce. 
Para ello dirigirse en el menú a Sales -> Order, "View" sobre la orden deseada (Esta debe haber sido realizada con TodoPago) y encontrará una sección con el título **Info**, dentro de esta hay 4 botónes diferentes para realizar la devolucion de TodoPago*.<br />
![imagen de configuracion](https://raw.githubusercontent.com/TodoPago/imagenes/master/nopcommerce/refund.PNG)</br>
Al realizar una devolucion parcial, se abrira un popUp en el cual se carga el monto a devolver.</br> 
![imagen de configuracion](https://raw.githubusercontent.com/TodoPago/imagenes/master/nopcommerce/partialRefund.PNG)</br>
<br />
[<sub>Volver a inicio</sub>](#inicio)

<a name="tablas"></a>
## Tablas de Referencia
######[Tabla de errores](#codigoerrores)

<a name="codigoerrores"></a>  

<table>		
<tr><th>Id mensaje</th><th>Mensaje</th></tr>				
<tr><td>1081</td><td>Tu saldo es insuficiente para realizar la transacción.</td></tr>
<tr><td>1100</td><td>El monto ingresado es menor al mínimo permitido</td></tr>
<tr><td>1101</td><td>El monto ingresado supera el máximo permitido.</td></tr>
<tr><td>1102</td><td>La tarjeta ingresada no corresponde al Banco indicado. Revisalo.</td></tr>
<tr><td>1104</td><td>El precio ingresado supera al máximo permitido.</td></tr>
<tr><td>1105</td><td>El precio ingresado es menor al mínimo permitido.</td></tr>
<tr><td>2010</td><td>En este momento la operación no pudo ser realizada. Por favor intentá más tarde. Volver a Resumen.</td></tr>
<tr><td>2031</td><td>En este momento la validación no pudo ser realizada, por favor intentá más tarde.</td></tr>
<tr><td>2050</td><td>Lo sentimos, el botón de pago ya no está disponible. Comunicate con tu vendedor.</td></tr>
<tr><td>2051</td><td>La operación no pudo ser procesada. Por favor, comunicate con tu vendedor.</td></tr>
<tr><td>2052</td><td>La operación no pudo ser procesada. Por favor, comunicate con tu vendedor.</td></tr>
<tr><td>2053</td><td>La operación no pudo ser procesada. Por favor, intentá más tarde. Si el problema persiste comunicate con tu vendedor</td></tr>
<tr><td>2054</td><td>Lo sentimos, el producto que querés comprar se encuentra agotado por el momento. Por favor contactate con tu vendedor.</td></tr>
<tr><td>2056</td><td>La operación no pudo ser procesada. Por favor intentá más tarde.</td></tr>
<tr><td>2057</td><td>La operación no pudo ser procesada. Por favor intentá más tarde.</td></tr>
<tr><td>2059</td><td>La operación no pudo ser procesada. Por favor intentá más tarde.</td></tr>
<tr><td>90000</td><td>La cuenta destino de los fondos es inválida. Verificá la información ingresada en Mi Perfil.</td></tr>
<tr><td>90001</td><td>La cuenta ingresada no pertenece al CUIT/ CUIL registrado.</td></tr>
<tr><td>90002</td><td>No pudimos validar tu CUIT/CUIL.  Comunicate con nosotros <a href="#contacto" target="_blank">acá</a> para más información.</td></tr>
<tr><td>99900</td><td>El pago fue realizado exitosamente</td></tr>
<tr><td>99901</td><td>No hemos encontrado tarjetas vinculadas a tu Billetera. Podés  adherir medios de pago desde www.todopago.com.ar</td></tr>
<tr><td>99902</td><td>No se encontro el medio de pago seleccionado</td></tr>
<tr><td>99903</td><td>Lo sentimos, hubo un error al procesar la operación. Por favor reintentá más tarde.</td></tr>
<tr><td>99970</td><td>Lo sentimos, no pudimos procesar la operación. Por favor reintentá más tarde.</td></tr>
<tr><td>99971</td><td>Lo sentimos, no pudimos procesar la operación. Por favor reintentá más tarde.</td></tr>
<tr><td>99977</td><td>Lo sentimos, no pudimos procesar la operación. Por favor reintentá más tarde.</td></tr>
<tr><td>99978</td><td>Lo sentimos, no pudimos procesar la operación. Por favor reintentá más tarde.</td></tr>
<tr><td>99979</td><td>Lo sentimos, el pago no pudo ser procesado.</td></tr>
<tr><td>99980</td><td>Ya realizaste un pago en este sitio por el mismo importe. Si querés realizarlo nuevamente esperá 5 minutos.</td></tr>
<tr><td>99982</td><td>En este momento la operación no puede ser realizada. Por favor intentá más tarde.</td></tr>
<tr><td>99983</td><td>Lo sentimos, el medio de pago no permite la cantidad de cuotas ingresadas. Por favor intentá más tarde.</td></tr>
<tr><td>99984</td><td>Lo sentimos, el medio de pago seleccionado no opera en cuotas.</td></tr>
<tr><td>99985</td><td>Lo sentimos, el pago no pudo ser procesado.</td></tr>
<tr><td>99986</td><td>Lo sentimos, en este momento la operación no puede ser realizada. Por favor intentá más tarde.</td></tr>
<tr><td>99987</td><td>Lo sentimos, en este momento la operación no puede ser realizada. Por favor intentá más tarde.</td></tr>
<tr><td>99988</td><td>Lo sentimos, momentaneamente el medio de pago no se encuentra disponible. Por favor intentá más tarde.</td></tr>
<tr><td>99989</td><td>La tarjeta ingresada no está habilitada. Comunicate con la entidad emisora de la tarjeta para verificar el incoveniente.</td></tr>
<tr><td>99990</td><td>La tarjeta ingresada está vencida. Por favor seleccioná otra tarjeta o actualizá los datos.</td></tr>
<tr><td>99991</td><td>Los datos informados son incorrectos. Por favor ingresalos nuevamente.</td></tr>
<tr><td>99992</td><td>La fecha de vencimiento es incorrecta. Por favor seleccioná otro medio de pago o actualizá los datos.</td></tr>
<tr><td>99993</td><td>La tarjeta ingresada no está vigente. Por favor seleccioná otra tarjeta o actualizá los datos.</td></tr>
<tr><td>99994</td><td>El saldo de tu tarjeta no te permite realizar esta operacion.</td></tr>
<tr><td>99995</td><td>La tarjeta ingresada es invalida. Seleccioná otra tarjeta para realizar el pago.</td></tr>
<tr><td>99996</td><td>La operación fué rechazada por el medio de pago porque el monto ingresado es inválido.</td></tr>
<tr><td>99997</td><td>Lo sentimos, en este momento la operación no puede ser realizada. Por favor intentá más tarde.</td></tr>
<tr><td>99998</td><td>Lo sentimos, la operación fue rechazada. Comunicate con la entidad emisora de la tarjeta para verificar el incoveniente o seleccioná otro medio de pago.</td></tr>
<tr><td>99999</td><td>Lo sentimos, la operación no pudo completarse. Comunicate con la entidad emisora de la tarjeta para verificar el incoveniente o seleccioná otro medio de pago.</td></tr>
</table>

<a name="availableversions"></a>
## Versiones Disponibles##
<table>
  <thead>
    <tr>
      <th>Version del Plugin</th>
      <th>Estado</th>
      <th>Versiones Compatibles</th>
    </tr>
  <thead>
  <tbody>
    <tr>
      <td><a href="https://github.com/TodoPago/nopcommerce-plugin/archive/master.zip">v1.0.0</a></td>
      <td>Stable (Current version)</td>
      <td>NopCommerce 3.70 <br />
          NopCommerce 3.80
      </td>
    </tr>
  </tbody>
</table>

*Click on the links above for instructions on installing and configuring the module.*


[<sub>Volver a inicio</sub>](#inicio)





