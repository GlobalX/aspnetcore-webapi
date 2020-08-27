# JMeter Load Tests

Load Test the app with JMeter

### Setup

1. Install JMeter and open the jmx file.
2. Enable/disable Modules depending on which crud operations you want to load test.
3. Play around with the threads/loops values as needed.
4. Run inside the gui, or to remove overhead, run in cli mode:

```
.\jmeter -n -t BooksAPI.jmx
```