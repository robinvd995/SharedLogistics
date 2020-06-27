USE [fmsD4PL]
GO
SELECT TRITItemDimensions.ID,
       TRITItemDimensions.DIMENSIONS_NO,
       TRITItemDimensions.ITEM_REF,
       TRITItemDimensions.INBOUNDID,
       HSCODE,
       PACKAGETYPE,
       GOODSDESCRIPTION,
       HEIGHT,
       WIDTH,
       LENGTH,
       WEIGHT,
       NETTOWEIGHT,
       VALUE,
       TRITItemDimensions.JOBNUMBER,
       TRITPurchaseOrder.CUSTOMSSTATUS,
       TRITPurchaseOrder.CUSTOMSTYPE,
       TRITPurchaseOrder.STATUS_DEFAULT,
       TRITItemDimensions.VALUEEUR,
       TRITItemDimensions.RATE,
       TRITItemDimensions.CURRENCYCODE,
       TRITItemDimensions.VATPERCENTAGE,
       TRITItemDimensions.VATAMOUNT,
       TRITItemDimensions.IMPORTDUTIESPERCENTAGE,
       TRITItemDimensions.IMPORTDUTIESAMOUNT,
       TRITItemDimensions.OTHERDESCRIPTION,
       TRITItemDimensions.OTHERPERCENTAGE,
       TRITItemDimensions.OTHERAMOUNT,
       TRITItemDimensions.LINEITEM,
       TRITItemDimensions.LINESTOCKAMOUNT,
       TRITItemDimensions.LINETOTALAMOUNT,
       TRITItemDimensions.LINECOUNTAMOUNT,
       TRITItemDimensions.[LOCATION],
       TRITItemDimensions.DANGEROUSGOODS,
       TRITItemDimensions.GOODSCONDITION,
       TRITItemDimensions.REMARKS,
       TRITItemDimensions.ARTICLE,
       TRITItemDimensions.QUANTITY,
       WEIGHT_DIV,
       LENGTH_DIV,
       WIDTH_DIV,
       HEIGHT_DIV,
       FLAG,
       FLAGREASON,
       REMARKS2
FROM TRITItemDimensions
INNER JOIN TRITPurchaseOrder ON TRITItemDimensions.INBOUNDID = TRITPurchaseOrder.INBOUNDID
WHERE TRITItemDimensions.INBOUNDID = {0}