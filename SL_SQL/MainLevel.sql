USE [fmsD4PL]
GO
WITH ResultSet AS
  (SELECT distinct(TRITPurchaseOrder.ID),
          TRITPurchaseOrder.PO_NUMBER,
          TRITPurchaseOrder.INBOUNDID,
          convert(varchar, TRITShipment.[DATE], 106) AS INBOUNDDATE,
          TRITShipment.SHIPMENT_REF_NO,
          TRITItem.PO_NUMBERS_NO,
          TRITItem.COLLIES,
          TRITItem.DIMENSIONS_NO AS DIMENSIONS_NO_PERITEM,
          TRITPurchaseOrder.DIMENSIONS_NO,
          TRITItem.KGS,
          TRITPurchaseOrder.LOCATION,
          TRITPurchaseOrder.STATUS_DEFAULT,
          TRITPurchaseOrder.CREATED_BY,
          TRITShipment.ITEMS_TOTAL,
          TRITShipment.POL,
          TRITShipment.POD,
          TRITShipment.VESSEL,
          TRITShipment.REMARKS,
          convert(varchar, TRITShipment.ETA_DATE, 106) AS ETA_DATE,
          TRITShipment.ETA_TIME,
          TRITPurchaseOrder.JOBNUMBER,
          TRITItem.COMMODITY,
          TRITPurchaseOrder.CUSTOMSSTATUS,
          UPPER(TRITPurchaseOrder.MRN) AS MRN,
          convert(varchar, TRITPurchaseOrder.MRNVALIDTO, 106) MRNVALIDTO,
          TRITShipment.SUPPLIER_NAME,
          TRITShipment.SUPPLIER_REF,
          TRITShipment.SUPPLIER_COUNTRY,
          TRITShipment.SUPPLIER_CITY,
          TRITShipment.CUSTOMER_NAME,
          TRITPurchaseOrder.RELATIONCODE,
          TRITPurchaseOrder.DEPOT,
          TRITPurchaseOrder.TTL,
          TRITPurchaseOrder.CURRENCY,
          TRITPurchaseOrder.CUSTOMSTYPE,
          TRITPurchaseOrder.INCOTERM,
          TRITPurchaseOrder.COUNTRYCODE,
          TRITPurchaseOrder.COUNTRYCODEDISPATCH,
          TRITItem.CRR,
          TRITItem.TOTALNETTOWEIGHT,
          TRITItem.TOTALVALUE,
          TRITItem.ITEM_REF,
          TRITPurchaseOrder.PREDEPARTURE,

     (SELECT ISNULL(SUM([WEIGHT]), 0)
      FROM TRITItemDimensions
      WHERE TRITPurchaseOrder.INBOUNDID = TRITItemDimensions.INBOUNDID) AS DIMWEIGHTPERPO,
          TRITPurchaseOrder.MARKEDASSINGLEPO,
          TRITShipment.REMARKS_INTERNAL,
          TRITShipment.AWB_NO_INCOMING,
          TRITPurchaseOrder.EDI_STATUS,
          TRITShipment.SHIPMENT_REF_NO_MASTER,
          convert(varchar, TRITShipment.SHIPMENT_DEADLINE, 106) AS SHIPMENT_DEADLINE,
          TRITShipment.SHIPMENT_DEADLINE_TEXT,
          TRITItem.DANGEROUSGOODS,
          TRITItem.UNCODE,
          TRITPurchaseOrder.COURIERCHARGES,
          TRITPurchaseOrder.CONDITIONOFGOODS,
          TRITPurchaseOrder.TRACKINGNUMBER,
          CASE
              WHEN len(TRITPurchaseOrder.TRACKINGNUMBER)>=20 THEN left(TRITPurchaseOrder.TRACKINGNUMBER, 20) + '..'
              ELSE TRITPurchaseOrder.TRACKINGNUMBER
          END AS TRACKINGNUMBERFORMATTED,
          TRITPurchaseOrder.DELIVERYBY,
          TRITPurchaseOrder.ORIGIN,
          TRITShipment.DELIVERY_CITY,
          convert(varchar, TRITShipment.COLLECTIONDATE, 106) AS COLLECTIONDATE,
          convert(varchar, TRITShipment.ACTUALCOLLECTIONDATE, 106) AS ACTUALCOLLECTIONDATE,
          TRITShipment.PICKUPCOSTS,
          TRITPurchaseOrder.DESTINATION,
          TRITPurchaseOrder.MINIMUMTEMP,
          TRITPurchaseOrder.MAXIMUMTEMP,
          TRITPurchaseOrder.LINEITEM,
          TRITPurchaseOrder.EXEMPTNO,
          TRITPurchaseOrder.TOUPLOAD,
          TRITPurchaseOrder.UPLOADED,
          convert(varchar, TRITPurchaseOrder.UPLOADLASTSENT_DATE, 106) AS UPLOADLASTSENT_DATE,
          CONVERT(VARCHAR(8), TRITPurchaseOrder.UPLOADLASTSENT_TIME, 108) AS UPLOADLASTSENT_TIME,
          TRITShipment.TRANSPORT_MODE,
          TRITShipment.TRANSPORT_MODE_TYPE,
          TRITShipment.HAZARDOUS,
          DENSE_RANK() OVER (
                             ORDER BY CASE
                                          WHEN TRITPurchaseOrder.INBOUNDID IS NULL THEN 0
                                          ELSE 1
                                      END,
                                      TRITPurchaseOrder.INBOUNDID DESC) AS RowNumber
   FROM TRITPurchaseOrder
   LEFT OUTER JOIN TRITItem ON TRITItem.ID = TRITPurchaseOrder.JOBNUMBER
   LEFT OUTER JOIN TRITShipment ON TRITItem.ID = TRITShipment.JOBNUMBER
   WHERE TRITPurchaseOrder.DEPOT = 'S4PL_LS'
     AND TRITPurchaseOrder.STATUS_DEFAULT = 'IN')
SELECT *
FROM ResultSet
WHERE RowNumber BETWEEN 0 AND 50
ORDER BY INBOUNDID DESC