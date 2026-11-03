namespace FiaeTrainer.Web.Updates;

/// <summary> Unterscheidet einen fehlenden Release von einem fehlgeschlagenen Abruf. </summary>
public enum UpdateCheckStatus { NotConfigured, Disabled, Checking, NoReleases, UpToDate, Available, Unavailable }
