<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" exclude-result-prefixes="msxsl mstns #default"
                id="expandGroupsTransform"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns:mstns="DataRepsDemoProtocolSpec"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt">

  <xsl:output method="xml" indent="yes" />

  <xsl:template match="*">
    <xsl:element name="{concat('xs:',local-name())}" >
      <xsl:for-each select="@*">
        <xsl:attribute name="{local-name()}">
          <xsl:value-of select="."/>
        </xsl:attribute>
      </xsl:for-each>
      <xsl:if test="count(./*)!=0">
        <xsl:apply-templates/>
      </xsl:if>
    </xsl:element>
  </xsl:template>

  <xsl:template match="/xs:schema" >
    <xs:schema id="MicroLearningSvc" elementFormDefault="qualified" targetNamespace="MicroLearningSvc" xmlns="MicroLearningSvc" xmlns:mstns="MicroLearningSvc">
      <xsl:apply-templates  />
    </xs:schema>
  </xsl:template>

  <xsl:template match="//xs:complexType//xs:group">
    <xsl:variable name="groupName" select="./@ref" />
    <xsl:apply-templates select="/xs:schema/xs:group[@name=$groupName]/*" />
  </xsl:template>

</xsl:stylesheet>
