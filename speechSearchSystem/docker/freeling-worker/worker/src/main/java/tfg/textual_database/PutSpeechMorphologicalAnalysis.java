package tfg.textual_database;

public class PutSpeechMorphologicalAnalysis {

    private String id;
    private String morphologicalAnalysisRaw;


    public PutSpeechMorphologicalAnalysis(String id, String morphologicalAnalysisRaw) {
        this.id = id;
        this.morphologicalAnalysisRaw = morphologicalAnalysisRaw;
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getMorphologicalAnalysisRaw() {
        return morphologicalAnalysisRaw;
    }

    public void setMorphologicalAnalysisRaw(String morphologicalAnalysisRaw) {
        this.morphologicalAnalysisRaw = morphologicalAnalysisRaw;
    }
}
