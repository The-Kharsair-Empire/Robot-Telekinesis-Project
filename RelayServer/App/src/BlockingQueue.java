import java.util.concurrent.LinkedBlockingDeque;


//this class the the task queue for workers in worker pool to access, it is thread safe so no any two threads in the thread pool will execute the same task
public class BlockingQueue extends LinkedBlockingDeque<String> {
    BlockingQueue(){
        super();
    }

    public boolean add(String pos) {
        return super.add(pos);
    }

    public String take() throws InterruptedException{
        return super.take();
    }
}