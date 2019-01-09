using System.IO;

public enum Subject { LanguageSystem, SimplificationSystem, NumerologySystem, DNASequenceSystem, InterestingNumbers };

public interface IPublisher
{
    void Subscribe(ISubscriber subscriber, Subject subject);
}

public interface ISubscriber
{
    void Notify(Subject subject, FileSystemEventArgs e);
}
